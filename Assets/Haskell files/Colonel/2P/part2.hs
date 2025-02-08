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
filterMissiles missiles = -- INPUT HERE --

-- PLAYER 2 CODE
-- write a function to isolate only the INACTIVE landmines!
filterLandmines :: [Landmine] -> [Landmine]
filterLandmines mines = -- INPUT HERE --

-- TEST CODE
randomGen :: Int -> Bool
randomGen i = False -- who cares lol the player's not gonna see this, the definition is just here to stop a compiler error

coordsGen :: Int -> Coords
coordsGen i = (1, 1) -- same principle

main :: IO()
main = do