## Algoritmo de Validação do CPF - Documentação

### Base Legal:
- **Receita Federal do Brasil** - Algoritmo oficial
- **Resolução RFB nº 1.800/2018** - Normas sobre CPF
- **Instrução Normativa RFB nº 1.548/2015** - Cadastro de Pessoas Físicas

### Algoritmo Matemático:

#### 1. Primeiro Dígito Verificador:
```
CPF: A B C D E F G H I - X Y
```

**Passo 1:** Multiplique cada dígito pelos pesos (10, 9, 8, 7, 6, 5, 4, 3, 2)
```
Soma = A×10 + B×9 + C×8 + D×7 + E×6 + F×5 + G×4 + H×3 + I×2
```

**Passo 2:** Calcule o resto da divisão por 11
```
Resto = Soma % 11
```

**Passo 3:** Determine o primeiro dígito verificador
```
Se Resto < 2: Primeiro DV = 0
Se Resto ≥ 2: Primeiro DV = 11 - Resto
```

#### 2. Segundo Dígito Verificador:
```
CPF: A B C D E F G H I X - Y
```

**Passo 1:** Multiplique cada dígito pelos pesos (11, 10, 9, 8, 7, 6, 5, 4, 3, 2)
```
Soma = A×11 + B×10 + C×9 + D×8 + E×7 + F×6 + G×5 + H×4 + I×3 + X×2
```

**Passo 2:** Calcule o resto da divisão por 11
```
Resto = Soma % 11
```

**Passo 3:** Determine o segundo dígito verificador
```
Se Resto < 2: Segundo DV = 0
Se Resto ≥ 2: Segundo DV = 11 - Resto
```

### Exemplo Prático - CPF: 111.444.777-35

#### Cálculo do 1º DV:
```
1×10 + 1×9 + 1×8 + 4×7 + 4×6 + 4×5 + 7×4 + 7×3 + 7×2
= 10 + 9 + 8 + 28 + 24 + 20 + 28 + 21 + 14
= 162

162 % 11 = 8
11 - 8 = 3 ✓ (primeiro dígito)
```

#### Cálculo do 2º DV:
```
1×11 + 1×10 + 1×9 + 4×8 + 4×7 + 4×6 + 7×5 + 7×4 + 7×3 + 3×2
= 11 + 10 + 9 + 32 + 28 + 24 + 35 + 28 + 21 + 6
= 204

204 % 11 = 6
11 - 6 = 5 ✓ (segundo dígito)
```

**Resultado:** 111.444.777-35 é **VÁLIDO** ✅

### Exemplo de CPF Inválido - CPF: 578.456.855-64

#### Cálculo do 1º DV:
```
5×10 + 7×9 + 8×8 + 4×7 + 5×6 + 6×5 + 8×4 + 5×3 + 5×2
= 50 + 63 + 64 + 28 + 30 + 30 + 32 + 15 + 10
= 322

322 % 11 = 3
11 - 3 = 8 ≠ 6 ❌
```

**O primeiro dígito deveria ser 8, mas foi informado 6**
**Resultado:** 578.456.855-64 é **INVÁLIDO** ❌

### Regras Adicionais:
1. **CPFs com todos os dígitos iguais são inválidos** (000.000.000-00, 111.111.111-11, etc.)
2. **Deve ter exatamente 11 dígitos** após normalização
3. **Aceita formatação** com pontos e hífen ou apenas números

### Fontes Oficiais:
- Receita Federal do Brasil
- Normas técnicas do sistema tributário brasileiro
- Documentação oficial do algoritmo de validação do CPF
