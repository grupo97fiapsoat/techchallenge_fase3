provider "aws" {
  region = "sa-east-1"
}

resource "aws_db_instance" "sqlserver" {
  identifier         = "fastfood-sqlserver"
  engine             = "sqlserver-ex"
  engine_version     = "15.00.4073.23.v1" # SQL Server Express 2019
  instance_class     = var.db_instance_class
  allocated_storage  = var.db_allocated_storage

  # credenciais
  username = var.db_username
  password = var.db_password

  # configs
  skip_final_snapshot  = true
  publicly_accessible  = true
  license_model        = "license-included"
  vpc_security_group_ids = [aws_security_group.sqlserver_sg.id]

  tags = {
    Name = "fastfood-rds-sqlserver"
  }
}

# Security group para liberar porta 1433
resource "aws_security_group" "sqlserver_sg" {
  name        = "fastfood-sqlserver-sg"
  description = "Permitir acesso externo ao SQL Server"
  vpc_id      = data.aws_vpc.default.id

  ingress {
    description = "SQL Server"
    from_port   = 1433
    to_port     = 1433
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"]
  }

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }
}
