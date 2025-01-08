using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using DiffMatchPatch;
using TMPro;

public enum Operation {
    DELETE, INSERT, EQUAL
}

public struct DiffWrapper : INetworkSerializable {
    public Operation operation;
    public string text;

    public DiffWrapper(Diff diff) {
        operation = (Operation) diff.operation;
        text = diff.text;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref operation);
        serializer.SerializeValue(ref text);
    }

    public Diff ToDiff() {
        return new Diff((DiffMatchPatch.Operation) operation, text);
    }
}

public class ScriptSync : NetworkBehaviour
{
    private TMP_InputField codeField;
    public float syncInterval;
    private float timer = 0f;
    private string codeShadow, codeText;
    private diff_match_patch dmp;

    // Start is called before the first frame update
    void Start()
    {
        codeField = GetComponentInChildren<TMP_InputField>();
        RefreshText();

        dmp = new diff_match_patch();
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsServer) {
            timer += Time.deltaTime;
            if (timer >= syncInterval) {
                timer = 0f;
                // need to implement mechanism here that makes sure client doesn't send diffs for previous script 
                // when server has already changed to new script, as that would be disastrous
                ClientInitiateSync();
            }
        }
    }

    void RefreshText() {
        codeText = GetComponent<CodeEditor>().CleanColorFormatting(codeField.text);
        codeShadow = codeText;
    }

    private void ShowDiffs(List<Diff> diffs) {
        foreach (var diff in diffs)
        {
            Debug.Log(diff.ToString());
        }
    }

    private void ShowPatches(List<Patch> patches) {
        foreach (var patch in patches)
        {
            Debug.Log(patch.ToString());
        }
    }

    void ClientInitiateSync() {
        // get most recent version of code
        codeText = GetComponent<CodeEditor>().CleanColorFormatting(codeField.text);

        List<Diff> clientEdits = dmp.diff_main(codeShadow, codeText);
        dmp.diff_cleanupEfficiency(clientEdits);

        codeShadow = codeText;

        // ShowDiffs(clientEdits);

        List<DiffWrapper> temp = new List<DiffWrapper>();
        foreach (var diff in clientEdits)
        {
            temp.Add(new DiffWrapper(diff));
        }

        ServerApplyClientEditsRpc(temp.ToArray());
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    void ServerApplyClientEditsRpc(DiffWrapper[] clientEditsArray) {
        // get a list of diffs again
        List<Diff> clientEdits = new List<Diff>();
        foreach (var diffWrapper in clientEditsArray)
        {
            clientEdits.Add(diffWrapper.ToDiff());
        }

        // apply edits to server shadow
        List<Patch> serverShadowPatches = dmp.patch_make(codeShadow, clientEdits);
        codeShadow = (string) dmp.patch_apply(serverShadowPatches, codeShadow)[0];

        // get most up-to-date version of server code, containing server-side edits
        codeText = GetComponent<CodeEditor>().CleanColorFormatting(codeField.text);

        // attempt to apply client edits to server text
        List<Patch> serverTextPatches = dmp.patch_make(codeText, clientEdits);

        ShowPatches(serverTextPatches);

        codeText = (string) dmp.patch_apply(serverTextPatches, codeText)[0];

        codeField.text = codeText; // I would hope the comment highlighting gets automatically applied here

        // now, get server-side edits
        List<Diff> serverEdits = dmp.diff_main(codeShadow, codeText);
        dmp.diff_cleanupEfficiency(serverEdits);

        codeShadow = codeText;

        List<DiffWrapper> temp = new List<DiffWrapper>();
        foreach (var diff in serverEdits)
        {
            temp.Add(new DiffWrapper(diff));
        }
        ClientApplyServerEditsRpc(temp.ToArray());
    }

    [Rpc(SendTo.NotServer, RequireOwnership = false)]
    void ClientApplyServerEditsRpc(DiffWrapper[] serverEditsArray) {
        // get list of diffs again
        List<Diff> serverEdits = new List<Diff>();
        foreach (var diffWrapper in serverEditsArray) {
            serverEdits.Add(diffWrapper.ToDiff());
        }
 
        // apply server edits to client shadow
        List<Patch> clientShadowPatches = dmp.patch_make(codeShadow, serverEdits);
        codeShadow = (string) dmp.patch_apply(clientShadowPatches, codeShadow)[0];

        // get most up-to-date version of server code, containing server-side edits
        codeText = GetComponent<CodeEditor>().CleanColorFormatting(codeField.text);

        // attempt to apply server edits to client text
        List<Patch> clientTextPatches = dmp.patch_make(codeText, serverEdits);
        codeText = (string) dmp.patch_apply(clientTextPatches, codeText)[0];

        codeField.text = codeText;
    }
}
