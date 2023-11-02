type Size = Int
type Damage = Int
type Target = String

data Bullet = Bullet Size Damage Target

retarget :: Bullet -> Target -> Bullet
retarget (Bullet s d _) t = Bullet s d t

-- write a function to retarget all bullets in a list!
retargetAll :: [Bullet] -> Target -> [Bullet]


-- TEST CODE
main :: IO()
main = do
  let bullets = [Bullet 10 5 "player", Bullet 5 10 "player", Bullet 20 1 "player"]
  let newBullets = retargetAll bullets "enemy"
  let retargetCheck = and [t == "enemy" | (Bullet _ _ t) <- newBullets]
  print(retargetCheck)
  -- [Bullet 10 5 "friend", Bullet 5 10 "friend", Bullet 20 1 "friend"]