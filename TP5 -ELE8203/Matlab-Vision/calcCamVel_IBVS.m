function twist = calcCamVel_IBVS(p,pd,Z,f)
% Implémentation de la loi de commande pour IBVS
% p est une matrice 2 x n contenant les coordonnées image des n points
% saillant
% le resutat twist est [omega,v] pour la caméra 


x=p(1,:);
    y=p(2,:);
    n=length(x);
    L=zeros(2*n,6);
    K=3*eye(2*n);
    e=zeros(2*n,1);
    j=0;
    for i=1:2:2*n
        j=j+1;
        L(i:i+1,:)=interactionMat(x(j),y(j),Z(j),f);
        e(i)=pd(1,j)-p(1,j);
        e(i+1)=pd(2,j)-p(2,j);
    end
  
    inverseL=pinv(L);
    twist = inverseL*K*e;  % à compléter

end

