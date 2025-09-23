output "signer_service_account_email" {
  value       = var.use_signed_url ? google_service_account.gcs_signer[0].email : null
  description = "The email of the service account used for signing URLs."
}

output "name" {
  value = google_storage_bucket.main.name
}

output "hmac_access_key" {
  value = google_storage_hmac_key.key[0].access_id
}

output "hmac_secret_key_secret_manager_id" {
  value = google_secret_manager_secret.hmac_secret_key[0].id
}
