#!/bin/bash
# Nicks utils
nick-help() {
    echo "AVAILABLE COMMANDS:"
    printf "\t- update-all-pips\n"
    printf "\t- get-pod-versions\n"
    printf "\t- pfmongo\n"
    printf "\t- restart-sf-conn\n"
    printf "\t- restart-sfdc-streamer\n"
    printf "\t- lookup-payer\n"
    printf "\t- get-mongo-creds\n"
    printf "\t- download-logs\n"
    printf "\t- local-dev\n"
    printf "\t- get-otto-actions\n"
}

update-all-pips() {
    display-help() {
        printf "Updates all pip packages in the current environment.\n\n"
        printf "Usage: update-all-pips [[[--url pypi_url] [(-u | --username) repo_username] [(-p | --password) repo_password]] | --cogb]\n"
        printf "\t--cogb\t\tUses the 'COGBUS_PYPI_USERNAME' and 'COGBUS_PYPI_PASSWORD' environment variables to login to the 'pypi.cogb.us/simple' PyPI repo."
    }
    url=""
    username=""
    password=""
    while [[ $1 != "" ]]; do
        case $1 in
            --url) shift
                url=$1
                shift
                ;;
            -u | --username) shift
                username=$1
                shift
                ;;
            -p | --password) shift
                password=$1
                shift
                ;;
            --cogb) shift
                if [[ -z ${COGBUS_PYPI_USERNAME} || -z ${COGBUS_PYPI_PASSWORD} ]]; then
                    echo "Cogbus PyPI repo environment variables not set."
                    return 1
                fi
                username=${COGBUS_PYPI_USERNAME}
                password=${COGBUS_PYPI_PASSWORD}
                url="pypi.cogb.us/simple"
                ;;
            *)
                printf "Command '$1' is invalid\n"
                display-help
                return 1
                shift
        esac
    done

    login_str=""
    if [[ $username != "" ]]; then
        login_str="$username:$password@"
    fi

    if [[ $url != "" ]]; then
        pip install -U --index-url "https://${login_str}${url}" $(pip freeze | awk '{split($1, a, "=="); print a[1]}')
    else
        pip install -U $(pip freeze | awk '{split($1, a, "=="); print a[1]}')
    fi
}

get-pod-versions() {
    print-help() {
        echo "Prints image name and version for each K8s pod in the current context."
        printf "USAGE:\n\tget-pod-versions"
    }
    while [[ $1 != "" ]]; do
        case $1 in
            *)
                printf "\e[31mThis command takes no arguments.\e[0m\n\n"
                print-help
                return
                ;;
        esac
    done
    bash ~/get_pod_versions.sh
}

pfmongo() {
    bash ~/pfmongo.sh $@
}

restart-sf-conn() {
    bash ~/delete-sf-conn.sh
}

restart-sfdc-streamer() {
    bash ~/restart-sfdc-streamer.sh $@
}

lookup-payer() {
    print-help() {
        printf "Lookup a payer by ID in mongo."
        printf "\tUSAGE: lookup-payer [ (-p | --port port) ] payer_id"
        printf "\tport\tThe port to look for a mongo connection on"
        printf "\tpayer_id\tThe numeric ID of the payer to lookup a name for"
    }

    port=27017
    payer_id=0
    while [[ $1 != "" ]]; do
        case $1 in
            -p | --port)
                shift
                port=$1
                shift
                ;;

            --*)
                print-help
                return 1
                ;;

            *)
                payer_id=$1
                shift
                ;;
        esac
    done

    mongo --port $port --eval "db.payer.findOne({_id:'$payer_id'}).payer_name" NavTool
}

get-mongo-creds() {
    print-help() {
        printf "Displays information necessary to get mongo db credentials for Coram QA"
        printf "\tUSAGE: get-mongo-creds"
    }

    while [[ $1 != "" ]]; do
        case $1 in
            *)
                print-help
                return 1
                ;;
        esac
    done

    helm status mongodb
}

restart-tcs-streamer() {
	print-help() {
		printf "Restarts the TCS Streamer pod in the current K8s cluster.\n\n"
		printf "USAGE: restart-tcs-streamer\n"
	}
	
	while [[ $1 != "" ]]; do
		case $1 in
			-h | --help)
				print-help
				return 0
				;;
			*)
				printf "\e[31mThis commadn does not take any arguments.\e[0m\n"
				print-help
				return 1
				;;
		esac
	done

	# Get current running instance of streamer
	current_streamer=$(kubectl get pods | grep tcs-streamer | awk '$3 == "Running" {print $1}')
	echo "Context is $(kubectl config current-context)"
	echo "Restarting $current_streamer"
	# Restart teh streamer
	kubectl delete pod $current_streamer > /dev/null &
	echo "[Subprocess ID is $!]"	
	# Show streamer logs
	bash ~/tcs-streamer.sh
}	

local-dev() {
    echo "Initiating Local Development terminal..."
    tmux source-file ~/.tmux/localdev_session
}

download-logs() {
    print-help() {
        printf "Downloads a log file from gcloud storage based on the given parameters."
        printf "\nUSAGE:\tdownload-logs [--time time]"
        printf "\n\tdownload-logs [-y | --year] [ -m | --month ] [ -d | --day ] [ -h | --hour ] [app]\n\n"
    }
    app="verification"
    yyy="2019"
    mm="05"
    dd="05"
    hh="00"

    while [[ $1 != "" ]]; do
        case $1 in
            --help)
                print-help
                return 0
                ;;
            --time)
                echo "Option not yet implemented."
                return 1
                ;;
            -y | --year)
                shift
                yyy=$1
                shift
                ;;
            -m | --month)
                shift
                mm=$1
                shift
                ;;
            -d | --day)
                shift
                dd=$1
                shift
                ;;
            -h | --hour)
                shift
                hh=$1
                shift
                ;;
            -* | --*)
                echo "Invalid option '$1'"
                print-help
                return 1
                ;;
            *)
                app=$1
                shift
                ;;
        esac
    done

    output_file="$app-logs_$yyy$mm$dd$hh.json"
    gsutil cp "gs://cvs-logs/$app/$yyy/$mm/$dd/$hh:00:00_$hh:59:59_S0.json" $output_file
    printf "\nFile save at: $output_file"
}

get-pods() {
    print-help() {
        printf "No help text available for this command at this time.\n\n"
    }
    search_val=""
    while [[ $1 != "" ]]; do
        case $1 in
            -h | --help)
                print-help
                ;;
            *)
                search_val=$1
                shift
                ;;
        esac
    done

    if [[ $search_val != "" ]]; then
        echo "$(kubectl get pods | grep -i $search_val | awk '{print $1}')"
    else
        echo "$(kubectl get pods | awk '{print $1}' | sed '1d')"
    fi
}

get-otto-actions() {
    username=""
    password=""
    id=""

    while [[ $1 != "" ]]; do
        case $1 in
            --username | -U)
                ;;
            --password | -P)
                ;;
            --help)
		    echo "Help text is not implemented yet. Sorry for the inconvenience."
                ;;
            *)
                id=$1
                shift
                ;;
        esac
    done

    if [[ $password != "" && $username != "" ]]; then
        echo "Not implemented"
    else
        mongo --eval "db.payer.findOne({_id:'$id'}).otto_actions" NavTool
    fi
}

restart-sf-conn() {
	bash ~/restart-sf-conn.sh
}
