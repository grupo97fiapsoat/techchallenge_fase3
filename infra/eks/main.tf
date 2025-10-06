resource "aws_eks_cluster" "this" {
  name     = "meu-eks"
  role_arn = aws_iam_role.eks_cluster.arn

  vpc_config {
    subnet_ids = [
      "subnet-005a1b270f8337f68", # AZ 1
      "subnet-0e3bc39f611579f08", # AZ 2
    ]
    endpoint_private_access = false
    endpoint_public_access  = true
  }
}

resource "aws_eks_node_group" "this" {
  cluster_name    = aws_eks_cluster.this.name
  node_group_name = "techchallenge-nodes"
  node_role_arn   = aws_iam_role.eks_node_role.arn
  subnet_ids      = data.aws_subnets.default.ids

  scaling_config {
    desired_size = 1
    max_size     = 2
    min_size     = 1
  }

  instance_types = ["t3.micro"]
}
