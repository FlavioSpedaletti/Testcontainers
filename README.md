# TestContainers

## Para rodar a aplicação, navegar até a pasta da Api via terminal e rodar o docker compose
- docker compose up
    - sobe uma imagem do postgres e do pgadmin
- docker compose -f docker-compose-full.yml up
    - sobe uma imagem da api (usa o Dockerfile por baixo dos panos), do postgres e do pgadmin
- docker compose -f docker-compose-viewer.yml up
    - sobe uma imagem do postgres e do adminer (pgadmin aparentemente mais simples)

## Para rodar os testes via testcontainer, navegar até a pasta dos testes via terminal e rodar *dotnet test* (testcontainer aparentemente não funciona se rodar os testes pela IDE)
- o teste sobe roda aplicação via *WebApplicationFactory* e uma imagem docker do postgres, e então usa o httpClient para fazer as chamadas à api