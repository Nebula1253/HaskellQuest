data Fractal = Kowalewski | Fractal (Fractal)

-- Dr. Fractal is being subjected to an infinite recursion; help him!
createFractal :: Int -> Fractal
createFractal n = Fractal (createFractal (n-1))

-- Once that's done, write a function to save Kowalewski from the infinite recursion
saveFractal :: Fractal -> Fractal
-- INPUT HERE --

-- TEST CODE
main :: IO()
main = do
    let fractal = createFractal 4
    let createCheck = fractal == Fractal (Fractal (Fractal (Fractal Kowalewski)))

    let savedCheck = saveFractal fractal == Kowalewski

    if createCheck
        then
            if savedCheck
                then
                    print("True")
                else
                    print("error: Kowalewski is still stuck in an infinite recursion!")
        else
            print("error: Something's wrong with the createFractal function!")