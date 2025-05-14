prep:
	docker compose up -d

run:
	dotnet run

yac:
	httpyac send $(NAME).http --all --env=local

play:
	bash -c -i 'make run & sleep 5 && make yac && fg'