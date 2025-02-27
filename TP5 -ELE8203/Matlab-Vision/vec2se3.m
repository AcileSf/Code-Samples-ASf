function xi = vec2se3(v)
% transforme un vecteur en une matrice de se(3)
% v est le vecteur [omega,vel], dans cet ordre
xi = [0 -v(3) v(2) v(4); v(3) 0 -v(1) v(5); -v(2) v(1) 0 v(6); 0 0 0 0];
end

