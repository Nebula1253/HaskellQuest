type Position = Int
type ID = String

type Target = String
type Angle = Float

type Coords = (Int, Int)

data Missile = Missile ID Position

launch :: Int -> [Missile]
launch n = [Missile (idGen i) i | i <- [1..n]]

data HomingMissile = HomingMissile Target Angle

launchHoming :: Target -> Int -> [HomingMissilee]
launchHoming t n = [HomingMissile t (angleGen i)| i <- [1..n]]

data Landmine = Landmine ID Coords

set :: Int -> [Landmine]
set n = [Landmine (coordsGen i) (randomGen i) | i <- [1..n]]

data FreezeMissile = FreezeMissile Coords Angle

launchFreeze :: Coords -> Int -> FreezeMissile


-- PLAYER 1 CODE
-- write a function to make the homing missiles target
-- one of the regular missiles each, so they wipe
-- each other out!

retarget :: HomingMissile -> Target -> HomingMissile
retarget (HomingMissile _ a) t = HomingMissile t a

backfire :: [Missile] -> [HomingMissile] -> [HomingMissile]
backfire m_list hm_list = -- INPUT HERE --

-- PLAYER 2 CODE
-- write a function to make the freeze missiles
-- target one landmine each, so they freeze 
-- the mines solid!

resetAim :: FreezeMissile -> Coords -> FreezeMissile
resetAim (FreezeMissile _ a) = FreezeMissile c a

freezeMines :: [Landmine] -> [FreezeMissile] -> [FreezeMissile]
freezeMines mines fm_list = -- INPUT HERE --

-- TEST CODE