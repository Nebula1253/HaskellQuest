-- Code snippet starts here
type Position = Int

data DropMissile = DropMissile Position Bool -- the bool is whether the missile is active or not

launchMissiles :: Int -> [DropMissile]
launchMissiles nrMissiles = [DropMissile i (randomGen i) | i <- [1..nrMissiles]]
-- don't worry about randomGen, it's just a function that randomly decides if the missile is active or not

-- write a function to filter out the missiles that are INACTIVE, and then highlight them: we'll be using this to interfere with the attack!
-- You'll have to use the function highlight :: DropMissile -> DropMissile here; don't worry about how it works, just make sure you highlight the right missiles
highlightDummies :: [DropMissile] -> [DropMissile]
highlightDummies missiles = -- INPUT HERE --


-- TEST CODE
randomGen :: Int -> Bool
randomGen i = False -- who cares lol the player's not gonna see this, the definition is just here to stop a compiler error

highlight :: DropMissile -> DropMissile
highlight missile = missile -- who cares lol the player's not gonna see this, the definition is just here to stop a compiler error

main :: IO()
main = do
    let missiles = [DropMissile 1 True, DropMissile 2 False, DropMissile 3 True, DropMissile 4 True]
    let newMissiles = highlightDummies missiles
    let highlightCheck = and [b == False | (DropMissile _ b) <- newMissiles]
    print(highlightCheck)
