type Position = Int
type ID = String

type Target = String
type Angle = Float

type Coords = (Int, Int)

data Missile = Missile ID Position

launch :: Int -> [Missile]
launch n = [Missile (idGen i) i | i <- [1..n]]

data HomingMissile = HomingMissile Target Angle

launchHoming :: Target -> Int -> [HomingMissile]
launchHoming t n = [HomingMissile t (angleGen i)| i <- [1..n]]

data Landmine = Landmine Coords

setMines :: Int -> [Landmine]
setMines n = [Landmine (coordsGen i) | i <- [1..n]]

data FreezeMissile = FreezeMissile Coords Angle

launchFreeze :: Coords -> Int -> [FreezeMissile]
launchFreeze c n = [FreezeMissile c (angleGen i) | i <- [1..n]]

-- PLAYER 1 CODE
-- write a function to make the homing missiles target
-- one of the regular missiles each, so they wipe
-- each other out!

retarget :: HomingMissile -> Target -> HomingMissile
retarget (HomingMissile _ a) t = HomingMissile t a

backfire :: [Missile] -> [HomingMissile] -> [HomingMissile]
backfire m_list hm_list = undefined -- INPUT HERE --

-- PLAYER 2 CODE
-- write a function to make the freeze missiles
-- target one landmine each, so they freeze 
-- the mines solid!

resetAim :: FreezeMissile -> Coords -> FreezeMissile
resetAim (FreezeMissile _ a) c = FreezeMissile c a

freezeMines :: [Landmine] -> [FreezeMissile] -> [FreezeMissile]
freezeMines mines fm_list = undefined -- INPUT HERE --

-- TEST CODE
idGen :: Int -> ID
idGen x = "missile" ++ show x

coordsGen :: Int -> Coords
coordsGen x = (x, x)

angleGen :: Int -> Angle
angleGen x = 0.0

main :: IO()
main = do
    let dropMissiles = launch 5
    let homingMissiles = launchHoming "player" 5

    let freezeMissiles = launchFreeze (1, 1) 5
    let landmines = setMines 5

    let newHomingMissiles = backfire dropMissiles homingMissiles
    let newFreezeMissiles = freezeMines landmines freezeMissiles

    let homingCheck = and [t == t' | (HomingMissile t _) <- newHomingMissiles | (Missile t' _) <- dropMissiles]
                        && not (null newHomingMissiles)
                        && length newHomingMissiles == length homingMissiles
                        && and [a == a' | (HomingMissile _ a) <- homingMissiles | (HomingMissile _ a') <- newHomingMissiles]

    let freezeCheck = and [c == c' | (FreezeMissile c _) <- newFreezeMissiles | (Landmine c') <- landmines]
                        && not (null newFreezeMissiles)
                        && length newFreezeMissiles == length freezeMissiles
                        && and [a == a' | (FreezeMissile _ a) <- freezeMissiles | (FreezeMissile _ a') <- newFreezeMissiles]

    if homingCheck
        then
            if freezeCheck
                then do
                    print "True"
                    print "Additional: Both work"
                else do
                    print "error: The homing missile redirection works, but the freeze missile redirection doesn't!"
                    print "Additional: Only homing redirect works"
        else
            if freezeCheck
                then do
                    print "error: The freeze missile redirection works, but the homing missile redirection doesn't!"
                    print "Additional: Only freeze redirect works"
                else do
                    print "error: Neither function works as expected!"
                    print "Additional: Neither work"