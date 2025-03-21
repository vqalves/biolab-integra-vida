# BiopasIntegraVida - Instalação
Versão 1 (21/03/2025)

## Configuração
É necessário que a máquina possa rodar projeto .NET Core 8.0

## Publicação do projeto
Gerar binários através do comando `dotnet`. Os arquivos binários devem ser hospedados em algum servidor web, utilizando Windows/IIS, Linux/kestrel, Linux/nginx ou similares.
```bash
dotnet publish .\src\BiopasIntegraVida.Web\ -c Release
```

Ou gerar uma imagem através do Docker e hospedar em um sistema de conteiners.
```bash
docker build --no-cache .
```

## Criação de chave de API do Google Maps
Para criar uma chave de Google Maps nova, seguir as instruções oficiais do Google<br/>
https://developers.google.com/maps/documentation/javascript/get-api-key

A chave deve ser alimentada dentro do `appsettings.json`
```json
{
    "GoogleMaps": {
        "ApiKey": "chave_gerada"
    }
}
```

Na conta do Google, criar um Maps ID chamado `farmacias`, de acordo com as instruções oficiais do Google<br/>
https://developers.google.com/maps/documentation/javascript/map-ids/get-map-id

É importante que a conta do Google Maps esteja com o billing configurado. Caso contrário, no site poderá aparecer que o mapa não pôde ser carregado corretamente.

## Configuração do Google Analytics
Ao criar um novo projeto na conta de <a href="https://analytics.google.com/analytics/web/">Google Analytics</a>, copiar o identificador do projeto e colocar no atributo `GoogleAnalytics:Id` dentro do `appsettings.json`.

O identificador costuma ter um formato semelhante a `G-XXXXXXXXXX`.

```json
{
    "GoogleAnalytics": {
        "Id": "G-XXXXXXXXXX"
    }
}
```

## Configuração do sistema de logs de erro Sentry (opcional)
Se não desejar utilizar o Sentry, mantenha o atributo `Sentry:Dsn` em branco.

Para utilizar o Sentry, crie o projeto na <a href="https://sentry.io">plataforma Sentry</a>.

Após criação do projeto, dentro das configurações do projeto, acessar o menu `SDK Setup` -> `Client Keys (DSN)`.

Copiar o valor contido no campo DSN, e colocar no atributo `Sentry:Dsn` dentro do `appsettings.json`.

```json
{
    "Sentry": {
        "Dsn": "https://xxx@xxx.ingest.us.sentry.io/xxx"
    }
}
```

## Compressão de arquivos
O sistema trabalha com mecanismos internos de compressão (br, gzip) e minificação de arquivos estáticos, como javascript e CSS. A flag, contida no `appsettings.json` poder ser ativada ou desativada através do atributo `ApplicationConfig:CompressContent`, que deve ser setada como `true` ou `false`.

```json
{
    "ApplicationConfig": {
        "CompressContent": true
    }
}
```

A recomendação é utilizar `false` apenas na máquina de desenvolvimento, e todos os ambientes publicados manter como `true`.

## Mascaramento de produto
O sistema tem capacidade de customizar o nome do produto exibido baseado no EAN. As configurações estão no arquivo `appsettings.json`, atributo `InterPlayers:CustomProductNames`.

```json
{
    "InterPlayers": {
        "CustomProductNames": [
            { "name": "ATACAND 16MG", "ean": "7896206401146" },
            { "name": "ATACAND 8MG", "ean": "7896206401160" },
            { "name": "ATACAND HTC 16MG", "ean": "7896206402082" },
            { "name": "ATACAND HTC 8MG", "ean": "7896206402426" }
        ]
    }
}
```

O nome será substituído baseado no EAN do produto. Qualquer produto que for ser exibido em tela mas não estiver na lista do `InterPlayers:CustomProductNames` irá exibir o nome original do produto.

## Configuração por ambiente
Tanto as informações de autenticação como as informações de conexão com a API da InterPlayers estão configuradas dentro do arquivo `appsettings.json`.

```json
{
    "InterPlayers": {
    "AdB2c": {
        "TokenUrl": "<url_do_ambiente>",
        "Scope": "<url_do_ambiente>",
        "ClientId": "<hidden>",
        "ClientSecret": "<hidden>"
    },
    "Loyalty": {
        "AdministratorId": "021",
        "UrlConsultarUf": "<url_do_ambiente>",
        "UrlConsultarMedicamentos": "<url_do_ambiente>",
        "UrlConsultarEstabelecimentos": "<url_do_ambiente>",
        "UrlConsultarProfissional": "<url_do_ambiente>",
        "UrlConsultarPaciente": "<url_do_ambiente>",
        "UrlConsultarStatusLgpd": "<url_do_ambiente>",
        "UrlAceitarLgpd": "<url_do_ambiente>",
        "UrlCadastrarPaciente": "<url_do_ambiente>",
        "UrlAderirProduto": "<url_do_ambiente>"
    }
    }
}
```

Caso esteja hospedado em uma máquina virtual, o arquivo pode ser alterado sem necessidade de uma nova publicação.

Em casos de hospedagem em conteiners, uma imagem pra cada ambiente deve ser gerado.

É importante manter as URLs com atributos dentro dos colchetes, pois os dados serão sobrepostos pelo sistema de acordo com a funcionalidade.

Exemplo de JSON do appSettings.json
```json
{
  "AdministratorId": "021",
  "UrlConsultarEstabelecimentos": "https://domain/service/v2/Establishment/administrators/{administratorId}/types/MH/establishments?zip={zip}"
}
```

Resultado gerado pelo sistema ao consultar CEP `1234000`:
> https://domain/service/v2/Establishment/administrators/041/types/MH/establishments?zip=01234000

Tabela de atributos substituíveis por endpoint:
<table>
    <thead>
        <tr>
            <th>Endpoint</th>
            <th>Atributo</th>
            <th>Descrição</th>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td>UrlConsultarUf</td>
            <td>{administratorId}</td>
            <td>ID de administrador</td>
        </tr>
        <tr>
            <td>UrlConsultarMedicamentos</td>
            <td>{administratorId}</td>
            <td>ID de administrador</td>
        </tr>
        <tr>
            <td rowspan="2">UrlConsultarEstabelecimentos</td>
            <td>{administratorId}</td>
            <td>ID de administrador</td>
        </tr>
        <tr>
            <td>{zip}</td>
            <td>CEP do usuário</td>
        </tr>
        <tr>
            <td rowspan="3">UrlConsultarProfissional</td>
            <td>{administratorId}</td>
            <td>ID de administrador</td>
        </tr>
        <tr>
            <td>{crm}</td>
            <td>CRM do médico</td>
        </tr>
        <tr>
            <td>{uf}</td>
            <td>UF do médico</td>
        </tr>
        <tr>
            <td rowspan="2">UrlConsultarPaciente</td>
            <td>{administratorId}</td>
            <td>ID de administrador</td>
        </tr>
        <tr>
            <td>{cpf}</td>
            <td>CPF do paciente</td>
        </tr>
        <tr>
            <td rowspan="2">UrlConsultarStatusLgpd</td>
            <td>{administratorId}</td>
            <td>ID de administrador</td>
        </tr>
        <tr>
            <td>{userId}</td>
            <td>ID interno do paciente na InterPlayers</td>
        </tr>
        <tr>
            <td rowspan="2">UrlAceitarLgpd</td>
            <td>{administratorId}</td>
            <td>ID de administrador</td>
        </tr>
        <tr>
            <td>{userId}</td>
            <td>ID interno do paciente na InterPlayers</td>
        </tr>
        <tr>
            <td>UrlCadastrarPaciente</td>
            <td>{administratorId}</td>
            <td>ID de administrador</td>
        </tr>
        <tr>
            <td rowspan="2">UrlAderirProduto</td>
            <td>{administratorId}</td>
            <td>ID de administrador</td>
        </tr>
        <tr>
            <td>{userId}</td>
            <td>ID interno do paciente na InterPlayers</td>
        </tr>
    </tbody>
</table>