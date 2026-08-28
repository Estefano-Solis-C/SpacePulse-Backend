variable "namespace" {
  description = "Kubernetes namespace for SpacePulse"
  type        = string
  default     = "spacepulse-prod"
}

variable "environment" {
  description = "Deployment environment"
  type        = string
  default     = "production"
}
