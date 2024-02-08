-- Code snippet starts here
type Position = Int
type ID = String
type Target = String
type Angle = Float

data Missile = Missile ID Position
data HomingMissile = HomingMissile Target Angle 

launchMissiles :: Int -> [Missile]
launchMissiles nrMissiles = [Missile (idGen i) i | i <- [1..nrMissiles]]

launchHMissiles :: Target -> Int -> [HomingMissile]
launchHMissiles t nrMissiles = [HomingMissile t (angleGen i)| i <- [1..nrMissiles]]

retarget :: HomingMissile -> Target -> HomingMissile
retarget (HomingMissile _ a) t = HomingMissile t a

-- write a function to make the homing missiles target the regular missiles
backfire :: [Missile] -> [HomingMissile] -> [HomingMissile]
backfire missiles homingMissiles = -- INPUT HERE --

-- Code snippet ends here

-- TEST CODE
angleGen :: Int -> Float
angleGen i = 0.0 -- who cares lol the player's not gonna see this, the definition is just here to stop a compiler error

idGen :: Int -> String
idGen i = "missile" ++ show i

main :: IO()
main = do
    let missiles = launchMissiles 5
    let homingMissiles = launchHMissiles "player" 5
    let newHomingMissiles = backfire missiles homingMissiles
    let backfireCheck = and [t == "missile" ++ show i | (HomingMissile t _) <- newHomingMissiles | i <- [1..5]]
    print(backfireCheck)
