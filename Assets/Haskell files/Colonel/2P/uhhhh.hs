type Target = String
type Angle = Float

data Missile = Missile Target Angle
-- represents homing missiles; the angle is the starting 
-- angle of the missile

launch :: Target -> Int -> [Missile]
launch t n = [Missile t (angleGen i) | i <- [1..n]]
-- this launches n missiles at target t with random start angles

data FreezeMissile = FreezeMissile Bool
-- represents missiles that can freeze you in place; the bool is
-- whether the missile is active or not

launchFreeze :: Int -> [FreezeMissile]
launchFreeze n = [FreezeMissile True | i <- [1..n]]
-- this launches n missiles that can freeze you in place

-- PLAYER 1 CODE
-- write a function to retarget all Missiles
-- we'll be using this to fire them back at the enemy!
retarget :: [Missile] -> Target -> [Missile]
retarget missiles t = -- INPUT HERE --

-- PLAYER 2 CODE
-- write a function that deactivates the FreezeMissiles!
disable :: [FreezeMissile] -> [FreezeMissile]
disable missiles = -- INPUT HERE --

-- TEST CODE
angleGen :: Int -> Float
angleGen i = fromIntegral i -- the player's not going to see this, the definition is just here to stop a compiler error

main :: IO()
main = do
    let hMissiles = launch "player" 5
    let fMissiles = launchFreeze 5
    