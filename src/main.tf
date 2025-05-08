# terraform init 
# terraform plan -var="admin_password=<paswd>"
# terraform apply -var="admin_password=<paswd>"
terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "=3.0.0"
    }
  }
}
provider "azurerm" {
  features {}
}
variable "name" {
  description = "the name of the project"
  type        = string
  default     = "exportpro"
}
variable "admin_password" {
  description = "The admin password for the ssh"
  type        = string
  sensitive = true
}
resource "azurerm_resource_group" "example" {
  name     = "${var.name}-resources"
  location = "France Central"
}
resource "azurerm_network_security_group" "example" {
  name                = "acceptanceTestSecurityGroup1"
  location            = azurerm_resource_group.example.location
  resource_group_name = azurerm_resource_group.example.name
  security_rule {
    name                       = "ssh"
    priority                   = 300
    direction                  = "Inbound"
    access                     = "Allow"
    protocol                   = "Tcp"
    source_port_range          = "*"
    destination_port_range     = 22
    source_address_prefix      = "*"
    destination_address_prefix = "*"
  }
  security_rule {
    name                       = "exadeli"
    priority                   = 310
    direction                  = "Inbound"
    access                     = "Allow"
    protocol                   = "Tcp"
    source_port_range          = "*"
    destination_port_ranges     = [2000,8081,1200,27017]
    source_address_prefix      = "*"
    destination_address_prefix = "*"
  }
  security_rule {
    name                       = "AnyHttpOutbound"
    priority                   = 330
    direction                  = "Outbound"
    access                     = "Allow"
    protocol                   = "Tcp"
    source_port_range          = "*"
    destination_port_range     = 80
    source_address_prefix      = "*"
    destination_address_prefix = "*"
  }

  tags = {
    environment = "Production"
  }
}
resource "azurerm_virtual_network" "example" {
  name                = "${var.name}-network"
  address_space       = ["10.0.0.0/16"]
  location            = azurerm_resource_group.example.location
  resource_group_name = azurerm_resource_group.example.name
}
resource "azurerm_subnet" "example" {
  name                 = "internal"
  resource_group_name  = azurerm_resource_group.example.name
  virtual_network_name = azurerm_virtual_network.example.name
  address_prefixes     = ["10.0.2.0/24"]
}
resource "azurerm_subnet_network_security_group_association" "example" {
  subnet_id                 = azurerm_subnet.example.id
  network_security_group_id = azurerm_network_security_group.example.id
}
resource "azurerm_public_ip" "example" {
  name                = "${var.name}-public-ip"
  location            = azurerm_resource_group.example.location
  resource_group_name = azurerm_resource_group.example.name
  allocation_method   = "Dynamic"
  sku                 = "Basic"
} 
resource "azurerm_network_interface" "example" {
  name                = "${var.name}-nic"
  location            = azurerm_resource_group.example.location
  resource_group_name = azurerm_resource_group.example.name
  
  ip_configuration {
    name                          = "internal"
    subnet_id                     = azurerm_subnet.example.id
    private_ip_address_allocation = "Dynamic"
    public_ip_address_id          = azurerm_public_ip.example.id
  }
}
  
resource "azurerm_linux_virtual_machine" "example" {
  name                = "${var.name}-machine"
  resource_group_name = azurerm_resource_group.example.name
  location            = azurerm_resource_group.example.location

  size                = "Standard_D2s_v3"
  admin_username      = "adminuser"
  admin_password = var.admin_password
  disable_password_authentication = false
  network_interface_ids = [
    azurerm_network_interface.example.id,
  ]
  os_disk {
    caching              = "ReadWrite"
    storage_account_type = "Premium_LRS"
  }
  source_image_reference {


    publisher = "Canonical"
    offer     = "ubuntu-24_04-lts"
    sku       = "server"
    version   = "latest"
  }
}