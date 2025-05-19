output "public_ip" {
    description = "The public IP of the virtual machine"
    value = azurerm_public_ip.example.ip_address
}
output "sshName" {
    description = "The ssh name"
    value = azurerm_linux_virtual_machine.example.admin_username
}