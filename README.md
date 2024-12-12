# Lister

Another list app... 🤦‍♂️

PH=$(echo '' | docker run --rm -i datalust/seq config hash)

mkdir -p seq

docker run \
--name seq \
-d \
--restart unless-stopped \
-e ACCEPT_EULA=Y \
-e SEQ_FIRSTRUN_ADMINPASSWORDHASH="$PH" \
-v seq:/data \
-p 5341:80 \
datalust/seq

> _It keeps you runnin', yeah it keeps you runnin'_ - The Doobie Brothers