variable "project_id" {
  type = string
}

variable "bucket_name" {
  type = string
}

variable "location" {
  type = string
}

variable "storage_class" {
  type    = string
  default = "STANDARD"
}

variable "bucket_admins" {
  type    = list(string)
  default = []
}

variable "use_signed_url" {
  type    = bool
  default = false
}