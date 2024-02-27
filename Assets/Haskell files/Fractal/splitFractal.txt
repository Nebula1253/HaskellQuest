type Size = Int
type Power = Int

data Fractal = Fractal Size
data Laser = Laser Power

fire :: Fractal -> Laser
fire (Fractal n) = Laser (power n)
-- the power of the laser is based on the size of this Fractal

-- write a function to recursively halve Dr. Fractal into 
-- smaller Fractals who can do less damage!
split :: Fractal -> [Fractal]
split fractal = --INPUT HERE--

-- hint: the 'div' function in Haskell does integer division, 
-- i.e. div 7 2 == 3, not 3.5

-- TEST CODE
power :: Size -> Power
power n = n -- doesn't really matter

splitRef :: Fractal -> [Fractal]
splitRef (Fractal 1) = [Fractal 1]
splitRef (Fractal n) = splitRef (Fractal (n `div` 2)) ++ splitRef (Fractal (n `div` 2))

fireRef :: Fractal -> Laser
fireRef (Fractal n) = Laser n

instance Eq Laser where
    (Laser p1) == (Laser p2) = p1 == p2

instance Eq Fractal where
    (Fractal s1) == (Fractal s2) = s1 == s2

main :: IO()
main = do
    let splitF = split (Fractal 8)
    let splitFRef = splitRef (Fractal 8)

    let fireCheck = fire (Fractal 8) == fireRef (Fractal 8)
    if not fireCheck
        then
            print("error: You've modified the fire function! You can't do that!")
        else
            print(splitF == splitFRef)