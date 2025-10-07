using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Business;
using Entity;

namespace Lab07A2025
{
    public partial class MainWindow : Window
    {
        private Product? productoSeleccionado;
        private readonly BProduct businessProduct = new BProduct();

        public MainWindow()
        {
            InitializeComponent();
            CargarProductos();
            LimpiarFormulario();
        }

        private void CargarProductos()
        {
            try
            {
                var onlyActive = chkSoloActivos?.IsChecked == true ? true : (bool?)null;
                var products = businessProduct.Read(onlyActive);
                dataGrid.ItemsSource = products;
                lblStatus.Text = $"Total: {products.Count} productos";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar productos: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LimpiarFormulario()
        {
            txtProdName.Clear();
            txtPrice.Clear();
            txtStock.Clear();
            chkActive.IsChecked = true;
            productoSeleccionado = null;
            dataGrid.SelectedItem = null;
            btnEliminar.IsEnabled = false;
            btnGuardar.Content = "Guardar";
            lblStatus.Text = "Completa la información del producto";
            txtProdName.Focus();
        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            productoSeleccionado = dataGrid.SelectedItem as Product;
            if (productoSeleccionado != null)
            {
                txtProdName.Text = productoSeleccionado.Name ?? "";
                txtPrice.Text = productoSeleccionado.Price.ToString("F2", CultureInfo.InvariantCulture);
                txtStock.Text = productoSeleccionado.Stock.ToString();
                chkActive.IsChecked = productoSeleccionado.Active;
                btnGuardar.Content = "Actualizar";
                btnEliminar.IsEnabled = true;
                lblStatus.Text = $"Editando: {productoSeleccionado.Name}";
            }
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validaciones
                if (string.IsNullOrWhiteSpace(txtProdName.Text))
                {
                    MessageBox.Show("El nombre es obligatorio.", "Validación", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtProdName.Focus();
                    return;
                }

                if (!decimal.TryParse(txtPrice.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out var price) || price < 0)
                {
                    MessageBox.Show("Ingrese un precio válido.", "Validación", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtPrice.Focus();
                    return;
                }

                if (!int.TryParse(txtStock.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var stock) || stock < 0)
                {
                    MessageBox.Show("Ingrese un stock válido.", "Validación", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtStock.Focus();
                    return;
                }

                if (productoSeleccionado == null)
                {
                    // Crear nuevo
                    var product = new Product
                    {
                        Name = txtProdName.Text.Trim(),
                        Price = price,
                        Stock = stock,
                        Active = chkActive.IsChecked == true
                    };
                    businessProduct.Create(product);
                    MessageBox.Show("Producto creado exitosamente.", "Éxito", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    // Actualizar
                    productoSeleccionado.Name = txtProdName.Text.Trim();
                    productoSeleccionado.Price = price;
                    productoSeleccionado.Stock = stock;
                    productoSeleccionado.Active = chkActive.IsChecked == true;
                    businessProduct.Update(productoSeleccionado);
                    MessageBox.Show("Producto actualizado exitosamente.", "Éxito", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }

                CargarProductos();
                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnNuevo_Click(object sender, RoutedEventArgs e)
        {
            LimpiarFormulario();
        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (productoSeleccionado == null) return;

            var result = MessageBox.Show(
                $"¿Cómo deseas eliminar '{productoSeleccionado.Name}'?\n\n" +
                "Sí = Eliminación física (permanente)\n" +
                "No = Inactivar producto (eliminación lógica)\n" +
                "Cancelar = No eliminar",
                "Confirmar eliminación",
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Cancel) return;

            try
            {
                if (result == MessageBoxResult.Yes)
                {
                    // Eliminación física
                    var cascade = MessageBox.Show(
                        "¿Eliminar también los detalles relacionados?",
                        "Eliminación en cascada",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning) == MessageBoxResult.Yes;

                    var rows = businessProduct.HardDelete(productoSeleccionado.ProductId, cascade);
                    MessageBox.Show(
                        rows > 0 ? "Producto eliminado permanentemente." : "No se pudo eliminar el producto.",
                        "Resultado", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    // Eliminación lógica
                    var rows = businessProduct.Delete(productoSeleccionado.ProductId);
                    MessageBox.Show(
                        rows > 0 ? "Producto inactivado." : "No se pudo inactivar el producto.",
                        "Resultado", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                CargarProductos();
                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnReactivar_Click(object sender, RoutedEventArgs e)
        {
            if (productoSeleccionado == null)
            {
                MessageBox.Show("Selecciona un producto para reactivar.", "Información", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                businessProduct.Reactivate(productoSeleccionado.ProductId);
                MessageBox.Show("Producto reactivado exitosamente.", "Éxito", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
                CargarProductos();
                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtBuscador_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                var term = txtBuscador.Text?.Trim() ?? "";
                var onlyActive = chkSoloActivos?.IsChecked == true ? true : (bool?)null;
                var products = businessProduct.Read(onlyActive);

                if (!string.IsNullOrWhiteSpace(term))
                {
                    products = products.Where(p => 
                        p.Name?.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
                }

                dataGrid.ItemsSource = products;
                lblStatus.Text = $"Mostrando: {products.Count} productos";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error en búsqueda: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void chkSoloActivos_Checked(object sender, RoutedEventArgs e)
        {
            CargarProductos();
        }

        // Validaciones de entrada numérica
        private static readonly Regex RegexDecimal = new(@"^[0-9]*\.?[0-9]{0,2}$", RegexOptions.Compiled);
        private static readonly Regex RegexInteger = new(@"^[0-9]*$", RegexOptions.Compiled);

        private void Numeric_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = (TextBox)sender;
            var newText = textBox.Text.Insert(textBox.SelectionStart, e.Text);
            e.Handled = !RegexDecimal.IsMatch(newText);
        }

        private void Numeric_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(DataFormats.Text))
            {
                var text = (string)e.DataObject.GetData(DataFormats.Text);
                if (!RegexDecimal.IsMatch(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        private void Integer_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = (TextBox)sender;
            var newText = textBox.Text.Insert(textBox.SelectionStart, e.Text);
            e.Handled = !RegexInteger.IsMatch(newText);
        }

        private void Integer_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(DataFormats.Text))
            {
                var text = (string)e.DataObject.GetData(DataFormats.Text);
                if (!RegexInteger.IsMatch(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }
    }
}
