% Simulation de IBVS
% Utilise des fonctions de la toolbox de Peter Corke (machine vision)

clear;
clc;
close all;
% Création de la caméra, au centre du repère monde
% distance focale 15 mm, pixels de 10 micromètre de côté, point principal au
% centre du plan image
cam = CentralCamera('focal', 0.015, 'pixel', 10e-6, 'resolution', [1280 1024],...
                    'centre', [640 512], 'name', 'cameraTP5');

% Création de 4 points devant la caméra
P = mkgrid(2,0.2,'pose',SE3(0,0,1.0));   % points 1m devant la caméra

%% Calcul des images de ces points et affichage (en pixels)
cam.plot(P);
p0_pix = cam.project(P);
% Passage dans les coordonnées centrées au point principal
fmat = [cam.f 0 0;0 cam.f 0];
p0 = fmat * (cam.K\[p0_pix;ones(1,4)]);  % cam.K est la matrice de calibration de la caméra

%% Positions désirées des points images
 % Tcam_d = SE3.Rz(-deg2rad(180));   % pose désirée de la caméra - simple rotation autour de z
Tcam_d = SE3.Rz(-deg2rad(20))*SE3.Ry(deg2rad(10))*SE3.Rx(deg2rad(5));  % demandé dans le sujet
cam.plot(P, 'pose', Tcam_d);

% Coordonnées des points désirés - seule spécification nécessaire pour IBVS
pd_pix = cam.project(P, 'pose', Tcam_d);
pd = fmat * (cam.K\[pd_pix;ones(1,4)]);  % cam.K est la matrice de calibration de la caméra

%% Simulation de IBVS
nsteps = 1000;
dt = 0.01;  % période d'échantillonage 10 ms entre images
p = zeros(2,4,nsteps);  % positions des points sur l'image, en pixels
twist = zeros(6,nsteps); % torseurs cinématiques de la caméra 
p(:,:,1) = p0;
camPose = zeros(4,4,nsteps);
camPose(:,:,1) = double(cam.T);

Z = ones(1,4);
for k=1:nsteps-1
    twist(:,k) = calcCamVel_IBVS(p(:,:,k),pd,Z,cam.f);  % <--- fonction IBVS à compléter
    % on déplace la caméra par le twist pendant dt
    camPose(:,:,k+1) =  camPose(:,:,k)*expm(dt*vec2se3(twist(:,k)));  % <--- cette ligne est à compléter pour mettre à jour la pose
    cam.T = SE3(camPose(:,:,k+1));
    % décommenter la ligne suivante pour animer la vue de la caméra
    cam.plot(P); drawnow  
    % On mesure les points dans la nouvelle pose 
    p_pix = cam.project(P);
    % Passage dans les coordonnées centrées au point principal
    p(:,:,k+1)= fmat * (cam.K\[p_pix;ones(1,4)]);  
end

%% Plots
tf=1000*dt;
time=linspace(0,tf,nsteps);
s=(p(1,3,:)+p(1,2,:))/2;
s_x=zeros(1,nsteps);
s_y=zeros(1,nsteps);
s_z=zeros(1,nsteps);
for i=1:nsteps
    s_x(i)=camPose(1,4,i);
    s_y(i)=camPose(2,4,i);
    s_z(i)=camPose(3,4,i);
end    
  
figure();
grid on
hold on;
plot(time,s_x);
plot(time,s_y);
plot(time,s_z);
title("Coordonnées x, y, z dans s du centre de la caméra au cours du temps ",'FontSize',15)
xlabel('temps[s]','FontSize',10)
ylabel('x/y/z[m]','FontSize',10)
legend("x","y","z",'FontSize',15)
ax=gca;
ax.FontSize;
hold off;

angles=tr2rpy(camPose,'deg');
phi=zeros(1,nsteps);
psi=zeros(1,nsteps);
theta=zeros(1,nsteps);
for i=1:nsteps
    phi(i)=angles(i,1)-5;
    theta(i)=angles(i,2)-10;
    psi(i)=angles(i,3)+20;
end


figure();
grid on
hold on
plot(time,phi);
plot(time,theta);
plot(time,psi); 
title("Erreurs sur les angles ϕ, θ, ψ ",'FontSize',15)
xlabel('temps[s]','FontSize',10)
ylabel('erreur[deg]','FontSize',10)
legend("ϕ","θ","ψ",'FontSize',15)
ax=gca;
ax.FontSize;
hold off

erreur=zeros(1,nsteps);
e0=norm(pd-p(:,:,1));

for i=1:nsteps
    erreur(i)=norm(pd-p(:,:,i))/e0;   
end
figure();
grid on
hold on
plot(time,erreur);

title("Erreur image totale normalisée ",'FontSize',15)
xlabel('temps[s]','FontSize',10)
ylabel('erreur normalisée[-]','FontSize',10)
ax=gca;
ax.FontSize;
hold off

% Générez ici les graphes dont vous avez besoin

% % % % % % % AUTRE METHODE POUR LE GRAPH ERREUR DONNE APPROX LA MEME CHOSE% % % % % % % 
% erreur=zeros(1,nsteps);
% x=p(:,:,1);
%     y=p(:,:,1);
%     e=zeros(2*length(x),1);
%     k=0;
%     for j=1:2:2*length(x)
%         k=k+1;
%         e(j)=-p(1,k,1)+pd(1,k);
%         e(j+1)=-p(2,k,1)+pd(2,k);
%     end
%     e0=norm(e);  
% 
% for i=1:nsteps
%     x=p(:,:,i);
%     e=zeros(2*length(x),1);
%     pd=P(1:2,:);
%     k=0;
%     for j=1:2:2*length(x)
%         k=k+1;
%         e(j)=-p(1,k,i)+pd(1,k);
%         e(j+1)=-p(2,k,i)+pd(2,k);
%     end
%     erreur(i)=norm(e)/e0;   
% end
