terraform {
  required_version = "~> 1.13.3"

  required_providers {
    google = {
      source  = "hashicorp/google"
      version = "~> 7.3.0"
    }
    google-beta = {
      source  = "hashicorp/google-beta"
      version = "~> 7.3.0"
    }
  }

  // Location of state files.
  backend "gcs" {
    bucket = "candb-terraform"
    prefix = "global"
  }
}

provider "google" {
  project = local.project_id
  region  = local.project_region

  user_project_override = true
}

provider "google-beta" {
  project = local.project_id
  region  = local.project_region
}

data "google_project" "project" {
  project_id = local.project_id
}
