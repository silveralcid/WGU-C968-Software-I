﻿using C968_Software_I_CSharp.Models;
using System;
using System.Windows.Forms;

namespace C968_Software_I_CSharp.Forms
{
    public partial class AddProduct : Form
    {
        public Product Product { get; private set; }

        public AddProduct()
        {
            InitializeComponent();
            InitializeFormForAdd();  // Set up the form for adding a product
        }

        public AddProduct(Product product)
        {
            InitializeComponent();
            InitializeFormForModify(product);  // Set up the form for modifying a product
        }

        private void InitializeFormForAdd()
        {
            modifyProductLabel.Visible = false;
            addProductLabel.Visible = true;
            Product = new Product(); // Initialize Product as a new Product
        }

        private void InitializeFormForModify(Product product)
        {
            modifyProductLabel.Visible = true;
            addProductLabel.Visible = false;

            // Populate form fields with the existing product's data
            addProductIDTextBox.Text = product.ProductID.ToString();
            addProductNameTextBox.Text = product.ProductName;
            addProductInventoryTextBox.Text = product.ProductInventory.ToString();
            addProductPriceTextBox.Text = product.ProductPrice.ToString();
            addProductMinTextBox.Text = product.ProductMin.ToString();
            addProductMaxTextBox.Text = product.ProductMax.ToString();

            Product = product; // Assign the passed product to the Product property
        }

        private void AddProduct_Load(object sender, EventArgs e)
        {
            // Load all parts into the partsGridView
            partsGridView.DataSource = Inventory.FullParts;
            CustomizePartsGridView();

            // Initialize the associated parts grid with the current product's associated parts
            associatedPartsGridView.DataSource = Product.AssociatedParts;
            CustomizeAssociatedPartsGridView();
        }

        private void CustomizePartsGridView()
        {
            partsGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            partsGridView.ReadOnly = true;
            partsGridView.MultiSelect = false;
            partsGridView.AllowUserToAddRows = false;

            // Ensure columns have the correct headers
            partsGridView.Columns["PartID"].HeaderText = "Part ID";
            partsGridView.Columns["PartName"].HeaderText = "Name";
            partsGridView.Columns["PartInventory"].HeaderText = "Inventory";
            partsGridView.Columns["PartPrice"].HeaderText = "Price";
            partsGridView.Columns["PartMin"].HeaderText = "Min";
            partsGridView.Columns["PartMax"].HeaderText = "Max";

            partsGridView.RowHeadersVisible = false;
        }

        private void CustomizeAssociatedPartsGridView()
        {
            associatedPartsGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            associatedPartsGridView.ReadOnly = true;
            associatedPartsGridView.MultiSelect = false;
            associatedPartsGridView.AllowUserToAddRows = false;

            associatedPartsGridView.Columns["PartID"].HeaderText = "Part ID";
            associatedPartsGridView.Columns["PartName"].HeaderText = "Name";
            associatedPartsGridView.Columns["PartInventory"].HeaderText = "Inventory";
            associatedPartsGridView.Columns["PartPrice"].HeaderText = "Price";
            associatedPartsGridView.Columns["PartMin"].HeaderText = "Min";
            associatedPartsGridView.Columns["PartMax"].HeaderText = "Max";

            associatedPartsGridView.RowHeadersVisible = false;
        }

        private void addPartButton_Click(object sender, EventArgs e)
        {
            if (partsGridView.SelectedRows.Count > 0)
            {
                Part selectedPart = (Part)partsGridView.SelectedRows[0].DataBoundItem;
                Product.AddAssociatedPart(selectedPart);

                associatedPartsGridView.DataSource = null;
                associatedPartsGridView.DataSource = Product.AssociatedParts;
            }
            else
            {
                MessageBox.Show("Please select a part to add.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            CustomizePartsGridView();
            CustomizeAssociatedPartsGridView();
        }

        private void removePartButton_Click(object sender, EventArgs e)
        {
            if (associatedPartsGridView.SelectedRows.Count > 0)
            {
                int selectedIndex = associatedPartsGridView.SelectedRows[0].Index;
                Product.RemoveAssociatedPart(selectedIndex);

                associatedPartsGridView.DataSource = null;
                associatedPartsGridView.DataSource = Product.AssociatedParts;
            }
            else
            {
                MessageBox.Show("Please select a part to remove.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            CustomizePartsGridView();
            CustomizeAssociatedPartsGridView();
        }

        private void addProductSaveButton_Click_1(object sender, EventArgs e)
        {
            // Validate the inputs
            if (string.IsNullOrWhiteSpace(addProductIDTextBox.Text) ||
                string.IsNullOrWhiteSpace(addProductNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(addProductInventoryTextBox.Text) ||
                string.IsNullOrWhiteSpace(addProductPriceTextBox.Text) ||
                string.IsNullOrWhiteSpace(addProductMinTextBox.Text) ||
                string.IsNullOrWhiteSpace(addProductMaxTextBox.Text))
            {
                MessageBox.Show("All fields are required. Please fill in all fields.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!int.TryParse(addProductIDTextBox.Text, out int productID) ||
                !int.TryParse(addProductInventoryTextBox.Text, out int inventory) ||
                !decimal.TryParse(addProductPriceTextBox.Text, out decimal price) ||
                !int.TryParse(addProductMinTextBox.Text, out int min) ||
                !int.TryParse(addProductMaxTextBox.Text, out int max))
            {
                MessageBox.Show("Please enter valid numeric values for ID, Inventory, Price, Min, and Max.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (min > max)
            {
                MessageBox.Show("Min value cannot be greater than Max value.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (inventory < min || inventory > max)
            {
                MessageBox.Show("Inventory value must be between Min and Max.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            // Determine if this is an add or modify operation
            Product existingProduct = Inventory.LookupProduct(productID);

            if (Product == null || existingProduct == null)
            {
                // Adding a new product
                Product = new Product(productID, addProductNameTextBox.Text, inventory, price, min, max);

                // Add the product to the inventory
                Inventory.AddProduct(Product);
            }
            else
            {
                // Modifying an existing product
                existingProduct.ProductID = productID;
                existingProduct.ProductName = addProductNameTextBox.Text;
                existingProduct.ProductInventory = inventory;
                existingProduct.ProductPrice = price;
                existingProduct.ProductMin = min;
                existingProduct.ProductMax = max;

                // Update the product in the inventory
                Inventory.UpdateProduct(existingProduct);
            }

            // Close the form with OK result
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void addProductCancelButton_Click_1(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void productPartsSearchButton_Click(object sender, EventArgs e)
        {
            // Implement search functionality here if needed
        }
    }
}
