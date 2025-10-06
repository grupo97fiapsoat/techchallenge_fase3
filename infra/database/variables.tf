variable "db_username" {
  description = "Usuário administrador do SQL Server"
  type        = string
  default     = "sa"
}

variable "db_password" {
  description = "Senha do administrador do SQL Server"
  type        = string
  sensitive   = true
}

variable "db_name" {
  description = "Nome do banco de dados"
  type        = string
  default     = "FastFood"
}

variable "db_instance_class" {
  description = "Tipo da instância do banco"
  type        = string
  default     = "db.t3.micro" # compatível com Free Tier
}

variable "db_allocated_storage" {
  description = "Espaço em disco em GB"
  type        = number
  default     = 20
}
