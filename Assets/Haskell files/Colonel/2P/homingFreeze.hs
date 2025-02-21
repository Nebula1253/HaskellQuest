type Target = String
type Angle = Float

data HomingMissile = HomingMissile Target Angle
-- represents homing missiles; the angle is the starting 
-- angle of the missile

launch :: Target -> Int -> [HomingMissile]
launch t n = [HomingMissile t (angleGen i) | i <- [1..n]]
-- this launches n missiles at target t with random start angles

data FreezeMissile = FreezeMissile Bool Angle
-- represents missiles that can freeze you in place; the bool is
-- whether the missile is active or not

launchFreeze :: Int -> [FreezeMissile]
launchFreeze n = [FreezeMissile True (angleGen i) | i <- [1..n]]
-- this launches n missiles that can freeze you in place

-- PLAYER 1 CODE
-- write a function to retarget all HomingMissiles
-- we'll be using this to fire them back at the enemy!
retarget :: [HomingMissile] -> Target -> [HomingMissile]
retarget missiles t = -- INPUT HERE --

-- PLAYER 2 CODE
-- write a function that deactivates the FreezeMissiles
-- they shouldn't pose a problem after this!
disable :: [FreezeMissile] -> [FreezeMissile]
disable missiles = -- INPUT HERE --

-- TEST CODE
angleGen :: Int -> Float
angleGen i = fromIntegral i -- the player's not going to see this, the definition is just here to stop a compiler error

main :: IO()
main = do
    let hMissiles = launch "player" 5
    let fMissiles = launchFreeze 5

    let newHMissiles = retarget hMissiles "enemy"
    let retargetCheck = (and [t == "enemy" | (HomingMissile t _) <- newHMissiles]) && not (null newHMissiles) && (and [a == b | (HomingMissile _ a) <- newHMissiles | (HomingMissile _ b) <- hMissiles]) && (length hMissiles == length newHMissiles)
    let invalidTargets = [t | HomingMissile t _ <- newHMissiles, t /= "enemy"]

    let newFMissiles = disable fMissiles
    let freezeCheck = not (and [b | (FreezeMissile b) <- newFMissiles]) && not (null newFMissiles) && (length fMissiles == length newFMissiles) && (and [a == b | (FreezeMissile _ a) <- fMissiles | (FreezeMissile _ b) <- newFMissiles])

    if retargetCheck
        then
            if freezeCheck
                then do
                    print "True"
                    print "Additional: Both work"
                else do
                    print "error: The retarget function works, but the disable function doesn't!"
                    print "Additional: Only retarget works"
        else
            if freezeCheck
                then do
                    print "error: The disable function works, but the retarget function doesn't!"
                    print "Additional: Only disable works"
                else do
                    print "error: Neither function works as expected!"
                    print "Additional: Neither works"