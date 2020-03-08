#! /bin/bash

# Mongo port forwarding script
# -----------------------------------------

# TODO Delete once script is finished
# echo "Funciton not yet implemented -Nick"
# exit 1

help_text() {
    echo "Port forwards to the first mongodb pod with a status of 'Running' on the current cluster
    USage: bash pfmongo.sh [--port localport]"
}

port=27017
original_cluster=$(kubectl config current-context)
other_clusters=( $(kubectl config get-contexts | sed '1d' | awk '{print $1}') )
poc_cluster=""
prod_cluster=""
qa_cluster=""
uat_cluster=""
should_revert=true
for cluster in ${other_clusters[@]}; do
    case $cluster in
        *poc*)
            poc_cluster=$cluster
            ;;
        *prod*)
            prod_cluster=$cluster
            ;;
        *real-qa*)
            qa_cluster=$cluster
            ;;
        *real-uat*)
            uat_cluster=$cluster
            ;;
    esac
done
printf "Current cluster is ${original_cluster}\n"
# Determine local port
while [[ $1 != "" ]]; do
    case $1 in 
        -p | --port)    
            shift
            port=$1
            shift
            ;;
        poc)
            if [[ ${poc_cluster} != "" ]]; then
                kubectl config use-context ${poc_cluster}
            else
                printf "Already at desired K8s context.\n"
                should_revert=false
            fi
            shift
            ;;
        prod)
            if [[ ${prod_cluster} != "" ]]; then
                kubectl config use-context ${prod_cluster}
            else
                printf "Already at desired K8s context.\n"
                should_revert=false
            fi
            shift
            ;;
        qa)
            if [[ ${qa_cluster} != "" ]]; then
                kubectl config use-context ${qa_cluster}
            else
                printf "Already at desired K8s context.\n"
                should_revert=false
            fi
            shift
            ;;
        uat)
            if [[ ${uat_cluster} != "" ]]; then
                kubectl config use-context ${uat_cluster}
            else
                printf "Already at desired K8s context.\n"
                should_revert=false
            fi
            shift
            ;;
        *)
            help_text
            exit 1
            ;;
    esac
done
echo "Using port ${port}"
# Get the first running instance of mongodb pod
mongopod=$(kubectl get pods | grep mongo | awk '$3 == "Running" {print $1}')
echo "Using pod ${mongopod}"
printf "\n"
kubectl port-forward ${mongopod} ${port}:27017 &
sleep 2

if [[ $should_revert == true ]]; then
	echo "Reverting context"
	kubectl config use-context $original_cluster
fi

wait $!
