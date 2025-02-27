function L = interactionMat(x,y,Z,f)
%Renvoie la matrice d'interaction 2x6 pour un point saillant

L = [x*y/f -(x^2+f^2)/f y -f/Z 0 x/Z;
     (f^2+y^2)/f -x*y/f -x 0 -f/Z y/Z]; % à compléter

end

