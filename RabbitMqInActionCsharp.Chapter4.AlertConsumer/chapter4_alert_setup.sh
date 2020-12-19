#!/usr/bin/env bash

USER="alert_user"
PASSWORD="alertme"

rabbitmqctl add_user ${USER} ${PASSWORD}
rabbitmqctl set_permissions ${USER} ".*" ".*" ".*"

