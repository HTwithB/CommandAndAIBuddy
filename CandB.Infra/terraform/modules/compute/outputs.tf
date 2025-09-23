output "instance_id" {
  value       = google_compute_instance.main.id
  description = "The ID of the created instance."
}
