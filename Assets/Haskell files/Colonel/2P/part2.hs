type Position = Int

data Missile = Missile Position Bool 
-- represents a missile, the bool is whether the missile 
-- is active or not

launch :: Int -> [Missile]
launch n = [Missile i (randomGen i) | i <- [1..n]]
-- randomGen: a function that randomly decides if the missile 
-- is active or not; one missile per barrage won't hurt you

type Coords = (Int, Int)

data Landmine = Landmine Coords Bool
-- the bool is whether the landmine is active or not

set :: Int -> [Landmine]
set n = [Landmine (coordsGen i) (randomGen i) | i <- [1..n]]

-- PLAYER 1 CODE
-- write a function to isolate only the INACTIVE missiles!
filterMissiles :: [Missile] -> [Missile]
filterMissiles missiles = undefined -- INPUT HERE --

-- PLAYER 2 CODE
-- write a function to isolate only the INACTIVE landmines!
filterLandmines :: [Landmine] -> [Landmine]
filterLandmines mines = undefined -- INPUT HERE --

-- TEST CODE
randomGen :: Int -> Bool
randomGen i = False -- who cares lol the player's not gonna see this, the definition is just here to stop a compiler error

coordsGen :: Int -> Coords
coordsGen i = (1, 1) -- same principle

main :: IO()
main = do
    let missiles = [Missile 1 True, Missile 2 False, Missile 3 True, Missile 4 True]
    let landmines = [Landmine (1, 1) True, Landmine (2, 2) True, Landmine (3, 3) False, Landmine (4, 4) True]

    let newMissiles = filterMissiles missiles
    let newLandmines = filterLandmines landmines

    let missilesCheck = and [b == False | (Missile _ b) <- newMissiles] 
                        && not (null newMissiles) 
                        && and [a == b | (Missile a _) <- newMissiles | (Missile b x) <- missiles, not x] 

    let landminesCheck = and [b == False | (Landmine _ b) <- newLandmines] 
                        && not (null newLandmines) 
                        && and [a == b | (Landmine a _) <- newLandmines | (Landmine b x) <- landmines, not x] 

    if missilesCheck
        then
            if landminesCheck
                then do
                    print "True"
                    print "Additional: Both work"
                else do
                    print "error: Filtering missiles works, but filtering landmines doesn't!"
                    print "Additional: Only missile works"
        else
            if landminesCheck
                then do
                    print "error: Filtering landmines works, but filtering missiles doesn't!"
                    print "Additional: Only landmine works"
                else do
                    print "error: Neither function works as expected!"
                    print "Additional: Neither works"