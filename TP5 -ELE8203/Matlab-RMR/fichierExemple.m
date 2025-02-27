% Make sure to have the server side running in CoppeliaSim: 
% in a child script of a CoppeliaSim scene, add following command
% to be executed just once, at simulation start:
%
% simRemoteApi.start(19999)
%
% then start simulation, and run this program.
%
% IMPORTANT: for each successful call to simxStart, there
% should be a corresponding call to simxFinish at the end!

%---------
% JLN: ce fichier est basé sur simpleTest.m, fourni par CoppeliaSim
%---------

clear;
clc;

%% Initialisation des tableaux et variables
stepmax = 100000;
posRobot = zeros(stepmax,3);
currentstep = 1;

%%
disp('Program started');
% sim=remApi('remoteApi','extApi.h'); % using the header (requires a compiler)
sim=remApi('remoteApi');    % using the prototype file (remoteApiProto.m)
sim.simxFinish(-1);         % just in case, close all opened connections
clientID=sim.simxStart('127.0.0.1',19999,true,true,5000,5);

if (clientID>-1)
    disp('Connected to remote API server');
    
    % Now try to retrieve data in a blocking fashion (i.e. a service call):
    [res,objs]=sim.simxGetObjects(clientID,sim.sim_handle_all,sim.simx_opmode_blocking);
    if (res==sim.simx_return_ok)
        fprintf('Number of objects in the scene: %d\n',length(objs));
    else
        fprintf('# objects - Remote API function call returned with error code: %d\n',res);
    end
    
    % jln
    [returnCode,refPoint]=sim.simxGetObjectHandle(clientID,'ref_point',sim.simx_opmode_blocking);
    if (returnCode==sim.simx_return_ok)
        fprintf('Retrieved the handle on the ref point\n');
    else
        fprintf('Robot frame - Remote API function call returned with error code: %d\n',returnCode);
    end

    [returnCode,robotFrame]=sim.simxGetObjectHandle(clientID,'youBot_frame',sim.simx_opmode_blocking);
    if (returnCode==sim.simx_return_ok)
        fprintf('Retrieved the handle on the robot frame\n');
    else
        fprintf('Robot frame - Remote API function call returned with error code: %d\n',returnCode);
    end

    sim.simxGetObjectPosition(clientID,refPoint,robotFrame,sim.simx_opmode_streaming);  % Initialize streaming
    pause(0.5);
    while (currentstep <= stepmax)
        % Try to retrieve the streamed data
        [returnCode2,posRobot(currentstep,:)] = ...
            sim.simxGetObjectPosition(clientID,refPoint,robotFrame,sim.simx_opmode_buffer);

        % After initialization of streaming, it will take a few ms before the first value arrives, so check the return code
        if (returnCode2==sim.simx_return_ok)
            % debug line
            %fprintf('Position of the robot: %d\n',posRobot(currentstep,:));
            currentstep = currentstep+1;
            pause(0.00009);
        end
    end
    
    % La connection va aussi de Matlab à CoppeliaSim. Voici un exemple
    % Now send some data to CoppeliaSim in a non-blocking fashion:
    sim.simxAddStatusbarMessage(clientID,'Hello CoppeliaSim!',sim.simx_opmode_oneshot);
    % Par exemple, il est possible d'implémenter un contrôleur dans Matlab au lieu d'un script CoppeliaSim
    
    % Déconnexion
    % Before closing the connection to CoppeliaSim, make sure that the last command sent out had time to arrive. You can guarantee this with (for example):
    sim.simxGetPingTime(clientID);
    
    % Now close the connection to CoppeliaSim:
    sim.simxFinish(clientID);
else
    disp('Failed connecting to remote API server');
end
sim.delete(); % call the destructor!

disp('Program ended');

% Now look at and process the recorded data
xy=posRobot(:,1:2);
erreur=0;
err=zeros(1,stepmax);
for i=1:stepmax
   erreur=erreur+xy(i,1)^2+xy(i,2)^2;
   err(i)=sqrt(xy(i,1)^2+xy(i,2)^2);
end
steps=linspace(0,9,stepmax);
erreur=sqrt(erreur/stepmax)

figure();
grid on
hold on
plot(steps,err,'LineWidth',1)
title("Variation de l'erreur de position ",'FontSize',15)
xlabel('temps[s]','FontSize',10)
ylabel('erreur[m]','FontSize',10)
ax=gca;
ax.FontSize;
hold off



figure();
grid on
hold on
plot(steps,xy(:,1),'LineWidth',1) 
title("Variation de l'erreur en x ",'FontSize',15)
xlabel('temps[s]','FontSize',10)
ylabel('erreur[m]','FontSize',10)
ax=gca;
ax.FontSize;
hold off

figure();
grid on
hold on
plot(steps,xy(:,2),'LineWidth',1) 
title("Variation de l'erreur en y ",'FontSize',15)
xlabel('temps[s]','FontSize',10)
ylabel('erreur[m]','FontSize',10)
ax=gca;
ax.FontSize;
hold off

% figure();
% plot(steps,xy(:,2))