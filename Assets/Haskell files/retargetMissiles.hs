-- Code snippet starts here
type Target = String

data Missile = Missile Target Bool -- the bool is whether the missile has been launched or not

prepareMissiles :: Target -> [Missile]
prepareMissiles t = [Missile t False | _ <- [1..5]]

launchAll :: Target -> [Missile]
launchAll t = [Missile t True | (Missile t _) <- prepareMissiles t]

-- write a function to retarget all missiles - we'll be using this to interfere with the launch!
-- NOTE: make sure you're not interfering with the launch bool
retargetAll :: [Missile] -> Target -> [Missile]
retargetAll missiles t = -- INPUT HERE --

-- Code snippet ends here

-- TEST CODE
main :: IO()
main = do
  let missiles = prepareMissiles "player"
  let newMissiles = retargetAll missiles "enemy"
  let retargetCheck = and [t == "enemy" | (Missile t b) <- newMissiles, (Missile _ b') <- missiles, b == b']
  print(retargetCheck)