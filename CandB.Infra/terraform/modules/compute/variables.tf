variable "project_id" {
  description = "The ID of the project in which to create the resources."
  type        = string
}

variable "instance_name" {
  description = "The name of the compute instance."
  type        = string
}

variable "zone" {
  description = "The zone to deploy the resources in."
  type        = string
  default     = "asia-northeast1-a"
}

variable "os_image" {
  type        = string
  default     = "ubuntu-os-cloud/ubuntu-2404-noble-amd64-v20250117"
  description = "os image: https://cloud.google.com/compute/docs/images/os-details?hl=ja#ubuntu_lts"
}

variable "service_account_email" {
  description = "The email of the service account to attach to the instance."
  type        = string
}

variable "bucket" {
  type = object({
    name                              = string
    hmac_access_key                   = string
    hmac_secret_key_secret_manager_id = string
  })
}
