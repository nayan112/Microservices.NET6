Formatting in VS CODE : Alt + Shift + F

Covered
1. Docker
2. Kubernates
3. Event Bus using RabbitMQ
4. 

Pending
1. Service discovery
2. Handle Null connection

Execution
1. Build image from the folder with dockerfile : docker build -t nayan112/platformservice .
2. Run the image: docker run -p 8080:80 -d nayan112/platformservice (disabled https redirection & swagger is enabled in dev only)
3. push the image : docker push nayan112/platformservice

4. Check id kubernetis is running: kubectl version
5. Navigate to the folder K8S and run: kubectl apply -f platforms-deployment.yaml
6. Check deployments: kubectl get deployments/kubectl get pods (need to push the image to docker hub first if missed) 
7. delete deployment: kubectl delete deployment platform-deployment 
8. Start services to enable access through node port: kubectl apply -f platforms-np-srv.yaml
9. Check service: kubectl get services
10. Get the external port of nodeport to connect to the pod
11. Refresh deployment/docker image: kubectl rollout restart deployment <depoyment_name>
12. To view pods running from all namespaces: (default is application namespace):  kubectl get namespaces; kubectl get deployments --namespace=ingress-nginx; kubectl get services --namespace=ingress-nginx
13. Configuration of Ingress Nginx container & Ingress Nginx load balancer: kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/controller-v1.1.2/deploy/static/provider/cloud/deploy.yaml
14. Deploy ingress service so that application can be accessed from outside: kubectl apply -f ingress-srv.yaml  
15. Ingress service needs a host name. Localhost doesn't work. So create an entry in host file similar to hostname in deployment file.
16. Create persistent view claim for MSSQL: kubectl apply -f local-pvc.yaml
17. Verify the claim: kubectl get pvc, kubectl get storageclass
18. Create a secret for db password: kubectl create secret generic mssql --from-literal=SA_PASSWORD="r00t.R00T"  
19. Deploy mssql: kubectl apply -f mssql-platform-deployment.yaml   check status with pods & deployments (ImagePullBackOff is a common error)
20. Setup of DB migration (not part of execution): run ef migration (dotnet ef migrations add initialmigration, need to disable in memory db & set db connection string)
21. Deploy rabbitmq queue manager & management portal, load balancer & cluster ip: kubectl apply -f rabbitmq-deployment.yaml (verify at http://localhost:15672)
22. gRPC setup done without https, needed in the server (platformservice) -- check kestrel config in prod configs

Help
1. Editing code in VSCode: Navigate to the particular project and type code -r <foldername>

Errors
1. Swagger fails to load : Fix: decoreate api methods with REST method decorators, HTTPGET, HTTPPUT
2. GetPlatformById method doesnt hit breakpoint/Swagger doesn't hit the method: Duplicate method if query string is not recognized. Set decorator as HTTPGET("Id=id")
3. Converting YAML to JSON: yaml: line 4: found character that cannot start any token: Yaml is CRAZY.. use spaces instead of tab
4. NodePort not working: Verify name in selector-> app & service deployment file (eg. in platforms-deployment.yaml & platforms-np-srv.yaml)
5. http port is not accessible: System restart or try changing the port no.
6. ImagePullBackOff : no network, no image, wrong image path
7. mssql image deployment error: port 1433 in host machine in use, password policy not in complience






Enabling & using local dashboard: Remote- coming..
1. Deploy dashboard: kubectl apply -f dashboard.yaml
2. Deploy an admin user: kubectl apply -f dashboard-adminuser.yaml
3. Get the token: kubectl -n kubernetes-dashboard get secret $(kubectl -n kubernetes-dashboard get sa/admin-user -o jsonpath="{.secrets[0].name}") -o go-template="{{.data.token | base64decode}}"
4. Run the dashboard proxy: kubectl proxy 
5. Application is available Application
http://localhost:8001/api/v1/namespaces/kubernetes-dashboard/services/https:kubernetes-dashboard:/proxy/.
6. Remove the admin ServiceAccount and ClusterRoleBinding.

kubectl -n kubernetes-dashboard delete serviceaccount admin-user
kubectl -n kubernetes-dashboard delete clusterrolebinding admin-user