variable "lb_name" {
  description = "The name of the load balancer."
  type        = string
}

variable "instance_ids" {
  description = "The IDs of the instances to attach to the load balancer."
  type        = list(string)
}

variable "zone" {
  description = "The zone where the instance is located."
  type        = string
}

variable "dns" {
  type = object({
    domain       = string
    managed_zone = string
  })
}
