-- Code snippet starts here
type Target = String
type Angle = Float

data Missile = Missile Target Angle -- represents homing missiles; the angle is the starting angle of the missile

launchMissiles :: Target -> Int -> [Missile]
launchMissiles t nrMissiles = [Missile t (angleGen i)| i <- [1..nrMissiles]]

-- write a function to retarget all missiles - we'll be using this to interfere with the launch!
retargetAll :: [Missile] -> Target -> [Missile]
retargetAll missiles t = -- INPUT HERE --

-- Code snippet ends here

-- TEST CODE
angleGen :: Int -> Float
angleGen i = 0.0 -- who cares lol the player's not gonna see this, the definition is just here to stop a compiler error

main :: IO()
main = do
  let missiles = launchMissiles "player" 5
  let newMissiles = retargetAll missiles "enemy"
  let retargetCheck = and [t == "enemy" | (Missile t _) <- newMissiles]
  print(retargetCheck)