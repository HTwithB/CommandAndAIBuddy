data "google_project" "project" {
  project_id = var.project_id
}

resource "google_storage_bucket" "main" {
  name                        = var.bucket_name
  location                    = var.location
  storage_class               = var.storage_class
  uniform_bucket_level_access = true
}

resource "google_storage_bucket_iam_member" "main" {
  for_each = toset(var.bucket_admins)

  bucket = google_storage_bucket.main.name
  member = each.value
  role   = "roles/storage.admin"
}

resource "google_service_account" "gcs_signer" {
  count        = var.use_signed_url ? 1 : 0
  account_id   = "gcs-url-signer"
  display_name = "Service Account for GCS Signed URL"
}

resource "google_storage_bucket_iam_member" "signer_can_view" {
  count  = var.use_signed_url ? 1 : 0
  bucket = google_storage_bucket.main.name
  role   = "roles/storage.objectViewer"
  member = "serviceAccount:${google_service_account.gcs_signer[0].email}"
}

resource "google_project_iam_member" "signer_can_create_token" {
  count   = var.use_signed_url ? 1 : 0
  project = data.google_project.project.project_id
  role    = "roles/iam.serviceAccountTokenCreator"
  member  = "serviceAccount:${google_service_account.gcs_signer[0].email}"
}

resource "google_storage_hmac_key" "key" {
  count                 = var.use_signed_url ? 1 : 0
  service_account_email = google_service_account.gcs_signer[0].email
}

resource "google_secret_manager_secret" "hmac_secret_key" {
  count     = var.use_signed_url ? 1 : 0
  secret_id = "gcs-hmac-secret-key"
  replication {
    auto {}
  }
}

resource "google_secret_manager_secret_version" "hmac_secret_key_version" {
  count       = var.use_signed_url ? 1 : 0
  secret      = google_secret_manager_secret.hmac_secret_key[0].id
  secret_data = google_storage_hmac_key.key[0].secret
}
