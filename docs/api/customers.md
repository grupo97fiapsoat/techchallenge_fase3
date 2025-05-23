## Customer API

### Criar um novo cliente

**POST** `/api/v1/customers`

Cria um novo cliente com os dados fornecidos.

**Request Body:**
```json
{
    "name": "string (3-100 caracteres)",
    "email": "string (email válido)",
    "cpf": "string (11 dígitos)"
}
```

**Response:**
- 201 Created: Cliente criado com sucesso
  - Location: /api/v1/customers/{id}
  - Body: CustomerDto
- 400 Bad Request: Validação falhou
  - Body: ProblemDetails com erro detalhado

### Buscar cliente por CPF

**GET** `/api/v1/customers/cpf/{cpf}`

Busca um cliente pelo CPF informado.

**Path Parameters:**
- cpf: string (11 dígitos)

**Response:**
- 200 OK: Cliente encontrado
  - Body: CustomerDto
- 400 Bad Request: CPF inválido
  - Body: ProblemDetails com erro detalhado
- 404 Not Found: Cliente não encontrado
