-- Code snippet starts here
type Target = String
type Angle = Float

data Missile = Missile Target Angle -- represents homing 
-- missiles; the angle is the starting angle of the missile

launch :: Target -> Int -> [Missile]
launch t n = [Missile t (angleGen i) | i <- [1..n]]
-- this launches n missiles at target t with random start angles

-- write a function to retarget all Missiles
-- we'll be using this to fire them back at the enemy!
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
  let wrongTargets = [t | Missile t _ <- newMissiles, t != "enemy"]
  if null wrongTargets
    then
      print("True")
    else if head wrongTargets == "player"
      then
        print("error: Missiles still target player")
      else
        print("error: " ++ head wrongTargets ++ " is not a valid target")