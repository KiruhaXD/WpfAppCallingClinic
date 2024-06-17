using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using WpfAppCallingApi.Models;
using System.Windows.Input;

namespace WpfAppCallingApi
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindowModel : Window
    {
        HttpClient client = new HttpClient();


        public MainWindowModel()
        {
            client.BaseAddress = new Uri("http://localhost:5096");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json")
                );
            InitializeComponent();


        }

        /*public DataTable Select(string selectSQL) // функция подключения к базе данных и обработка запросов
        {
            DataTable dataTable = new DataTable("dataBase");                // создаём таблицу в приложении
                                                                            // подключаемся к базе данных
            SqlConnection sqlConnection = new SqlConnection("server=DESKTOP-HIOKBS7;Trusted_Connection=Yes;DataBase=Drug_treatment_clinic;");
            sqlConnection.Open();                                           // открываем базу данных
            SqlCommand sqlCommand = sqlConnection.CreateCommand();          // создаём команду
            sqlCommand.CommandText = selectSQL;                             // присваиваем команде текст
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand); // создаём обработчик
            sqlDataAdapter.Fill(dataTable);                                 // возращаем таблицу с результатом
            return dataTable;
        }*/



        #region CONSUMPTION
        private void btnLoadConsumption_Click(object sender, RoutedEventArgs e)
        {
            this.GetConsumption();
        }

        private async void GetConsumption() 
        {
            try 
            {
                var response = await client.GetStringAsync("consumption");
                var consumption = JsonConvert.DeserializeObject<List<Consumption>>(response);
                dgConsumption.ItemsSource = consumption;
            }
            catch (Exception ex) 
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void dgConsumption_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private async void btnUpdateConsumption_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int consumptionId;
                if (!int.TryParse(txtConsumptionId.Text, out consumptionId))
                {
                    MessageBox.Show("Ошибка при обновлении потребления", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var updatedConsumption = new Consumption
                {
                    Id = consumptionId,
                    idDrug = Int32.Parse(txtIdDrug.Text),
                    idDepartment = Int32.Parse(txtIdDepartment.Text),
                    useDate = DateTime.Parse(txtUseDate.Text),
                    quantityUsed = txtQuantityUsed.Text,
                    idUser = Int32.Parse(txtIdUser.Text)
                };

                var json = JsonConvert.SerializeObject(updatedConsumption);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PutAsync($"consumption/{consumptionId}", content);
                response.EnsureSuccessStatusCode();

                MessageBox.Show("Потребление обновлено успешно!.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                btnLoadConsumption_Click(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void MenuItemDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgConsumption.SelectedItem != null)
                {
                    var selectedConsumption = (Consumption)dgConsumption.SelectedItem; // Замените YourConsumptionType на ваш тип данных

                    var response = await client.DeleteAsync($"consumption/{selectedConsumption.Id}");
                    response.EnsureSuccessStatusCode();

                    MessageBox.Show("Потребление успешно удалено!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    btnLoadConsumption_Click(sender, e);

                   
                }
                else
                {
                    MessageBox.Show("Выберите запись для удаления", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnAddConsumption_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtConsumptionId.Text) || (string.IsNullOrWhiteSpace(txtIdDrug.Text) || string.IsNullOrWhiteSpace(txtIdDepartment.Text) ||
                    string.IsNullOrWhiteSpace(txtUseDate.Text) || string.IsNullOrWhiteSpace(txtQuantityUsed.Text)))
                {
                    MessageBox.Show("Заполните все обязательные поля", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                int consumptionId;
                if (!int.TryParse(txtConsumptionId.Text, out consumptionId))
                {
                    MessageBox.Show("Ошибка при добавлении потребления", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var newConsumption = new Consumption
                {
                    Id = consumptionId,
                    idDrug = Int32.Parse(txtIdDrug.Text),
                    idDepartment = Int32.Parse(txtIdDepartment.Text),
                    useDate = DateTime.Parse(txtUseDate.Text),
                    quantityUsed = txtQuantityUsed.Text,
                    idUser = Int32.Parse(txtIdUser.Text)
                };

                /*if (IsPatientPhoneNumberExists(txtPhoneNumber.Text)) 
                {
                    MessageBox.Show("Пациент с таким же номером телефона уже существует", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }*/

                if (IsConsumptionIdExists(consumptionId)) 
                {
                    MessageBox.Show("Потребление с таким же идентификатором уже существует", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                /*if (!ValidatePhoneNumber(txtPhoneNumber.Text))
                {
                    MessageBox.Show("Неверный формат номера телефона", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }*/

                var json = JsonConvert.SerializeObject(newConsumption);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("consumption", content);
                response.EnsureSuccessStatusCode();

                MessageBox.Show("Потребление успешно добавлено", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                btnLoadConsumption_Click(sender, e);



            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /*public bool ValidatePhoneNumber(string phoneNumber)
        {
            // Паттерн для проверки формата номера телефона (примерный формат)
            string phonePattern = @"^\+\d{1,3}\s?\(\d{2,3}\)\s?\d{3}-\d{2}-\d{2}$";

            return Regex.IsMatch(phoneNumber, phonePattern);
        }*/

        /*private bool IsPatientPhoneNumberExists(string phoneNumber)
        {
            var existingPatients = (List<Patients>)dgPatient.ItemsSource;
            return existingPatients.Any(p => p.phoneNumber == phoneNumber);
        }*/

        private bool IsConsumptionIdExists(int consumptionId)
        {
            var existingConsumption = (List<Consumption>)dgConsumption.ItemsSource;
            return existingConsumption.Any(p => p.Id == consumptionId);
        }

        private void txtFilterIdDrug_TextChanged(object sender, TextChangedEventArgs e)
        {
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(dgConsumption.ItemsSource);
            if (collectionView != null)
            {
                collectionView.Filter = o =>
                {
                    if (o is Consumption consumption)
                    {
                        return consumption.idDrug.ToString().StartsWith(txtFilterIdDrug.Text, StringComparison.OrdinalIgnoreCase);
                    }
                    return false;
                };
            }
        }

        private void txtFilterIdDepartment_TextChanged(object sender, TextChangedEventArgs e)
        {
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(dgConsumption.ItemsSource);
            if (collectionView != null)
            {
                collectionView.Filter = o =>
                {
                    if (o is Consumption consumption)
                    {
                        return consumption.idDepartment.ToString().StartsWith(txtFilterIdDepartment.Text, StringComparison.OrdinalIgnoreCase);
                    }
                    return false;
                };
            }
        }

        private void txtFilterUseDate_TextChanged(object sender, TextChangedEventArgs e)
        {
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(dgConsumption.ItemsSource);
            if (collectionView != null)
            {
                collectionView.Filter = o =>
                {
                    if (o is Consumption consumption)
                    {
                        return consumption.useDate.ToString().StartsWith(txtFilterUseDate.Text, StringComparison.OrdinalIgnoreCase);
                    }
                    return false;
                };
            }
        }

        private void txtFilterQuantityUsed_TextChanged(object sender, TextChangedEventArgs e)
        {
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(dgConsumption.ItemsSource);

            if (collectionView != null)
            {
                collectionView.Filter = o =>
                {
                    if (o is Consumption consumption)
                    {
                        return consumption.quantityUsed.StartsWith(txtFilterQuantityUsed.Text, StringComparison.OrdinalIgnoreCase);
                    }
                    return false;
                };
            }
        }

        private void txtFilterIdUser_TextChanged(object sender, TextChangedEventArgs e)
        {
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(dgConsumption.ItemsSource);

            if (collectionView != null)
            {
                collectionView.Filter = o =>
                {
                    if (o is Consumption consumption)
                    {
                        return consumption.idUser.ToString().StartsWith(txtFilterIdUser.Text, StringComparison.OrdinalIgnoreCase);
                    }
                    return false;
                };
            }
        }

        private void txtEmail_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void btnStartFilter_Click(object sender, RoutedEventArgs e)
        {

        }

        private void txtSearchInGrid_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox != null)
            {
                ICollectionView view = CollectionViewSource.GetDefaultView(dgConsumption.ItemsSource);
                if (view != null)
                {
                    view.Filter = item =>
                    {
                        if (item is Consumption itemType)
                        {
                            return itemType.idDrug.ToString().Contains(textBox.Text) ||
                            itemType.idDepartment.ToString().Contains(textBox.Text) ||
                            itemType.useDate.ToString().Contains(textBox.Text) ||
                            itemType.quantityUsed.Contains(textBox.Text) ||
                            itemType.idUser.ToString().Contains(textBox.Text);
                        }
                        return false;
                    };
                }
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {

        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        #endregion

        #region DEPARTMENTS


private async void btnAddDepartments_Click(object sender, RoutedEventArgs e)
{
    try
    {
        if (string.IsNullOrWhiteSpace(txtDepartmentId.Text) || string.IsNullOrWhiteSpace(txtName.Text) ||
            string.IsNullOrWhiteSpace(txtResponsibleEmployee.Text))
        {
            MessageBox.Show("Заполните все обязательные поля", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        int departmentId;
        if (!int.TryParse(txtDepartmentId.Text, out departmentId))
        {
            MessageBox.Show("Ошибка при добавлении подразделения", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        var newDepartment = new Departments
        {
            Id = departmentId,
            name = txtName.Text,
            responsibleEmployee = txtResponsibleEmployee.Text
        };

        /*if (!ValidateEmail(txtContactInfo.Text))
        {
            MessageBox.Show("Неверный формат почты", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }*/

        var json = JsonConvert.SerializeObject(newDepartment);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await client.PostAsync("departments", content);
        response.EnsureSuccessStatusCode();

        MessageBox.Show("Подразделение успешно добавлено", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        btnLoadDepartments_Click(sender, e);
    }
    catch (Exception ex)
    {
        MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
    }
}

/*public bool ValidateEmail(string email)
{
    // Паттерн для проверки формата электронной почты
    string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

    return Regex.IsMatch(email, emailPattern);
}*/

        private async void btnLoadDepartments_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var response = await client.GetStringAsync("departments");
                var consumption = JsonConvert.DeserializeObject<List<Departments>>(response);
                dgDepartment.ItemsSource = consumption;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

private async void btnUpdateDepartments_Click(object sender, RoutedEventArgs e)
{
    try
    {
        int departmentId;
        if (!int.TryParse(txtDepartmentId.Text, out departmentId))
        {
            MessageBox.Show("Ошибка при обновлении подразделения", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        var updateDepartment = new Departments
        {
            Id = departmentId,
            name = txtName.Text,
            responsibleEmployee = txtResponsibleEmployee.Text
        };

        var json = JsonConvert.SerializeObject(updateDepartment);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await client.PutAsync($"departments/{departmentId}", content);
        response.EnsureSuccessStatusCode();

        MessageBox.Show("Подразделение успешно обновлено", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        btnLoadDepartments_Click(sender, e);
    }
    catch (Exception ex)
    {
        MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
    }
}
        private async void MenuItemDeleteDepartment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgDepartment.SelectedItem != null)
                {
                    var selectedConsumption = (Departments)dgDepartment.SelectedItem; // Замените YourConsumptionType на ваш тип данных

                    var response = await client.DeleteAsync($"departments/{selectedConsumption.Id}");
                    response.EnsureSuccessStatusCode();

                    MessageBox.Show("Подразделение успешно удалено!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    btnLoadDepartments_Click(sender, e);


                }
                else
                {
                    MessageBox.Show("Выберите запись для удаления", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void txtFilterName_TextChanged(object sender, TextChangedEventArgs e)
{
    ICollectionView collectionView = CollectionViewSource.GetDefaultView(dgDepartment.ItemsSource);

    if (collectionView != null) 
    {
        collectionView.Filter = o =>
        {
            if (o is Departments departments)
            {
                return departments.name.ToLower().StartsWith(txtFilterName.Text, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        };
    }
}

private void txtFilterResponsibleEmployee_TextChanged(object sender, TextChangedEventArgs e)
{
    ICollectionView collectionView = CollectionViewSource.GetDefaultView(dgDepartment.ItemsSource);

    if (collectionView != null)
    {
        collectionView.Filter = o =>
        {
            if (o is Departments departments)
            {
                return departments.responsibleEmployee.ToLower().StartsWith(txtFilterResponsibleEmployee.Text, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        };
    }
}

private void txtSearchInGridDepartments_TextChanged(object sender, TextChangedEventArgs e)
{
    TextBox textBox = sender as TextBox;

    if (textBox != null)
    {
        ICollectionView view = CollectionViewSource.GetDefaultView(dgDepartment.ItemsSource);
        if (view != null)
        {
            view.Filter = item =>
            {
                if (item is Departments itemType)
                {
                    return itemType.name.ToLower().Contains(textBox.Text) ||
                    itemType.responsibleEmployee.ToLower().Contains(textBox.Text);
                }
                return false;
            };
        }
    }
}

#endregion

        #region DRUGS
private async void btnAddDrug_Click(object sender, RoutedEventArgs e)
{

    try
    {
        if (string.IsNullOrWhiteSpace(txtDrugId.Text) || string.IsNullOrWhiteSpace(txtNameDrug.Text) || 
                    string.IsNullOrWhiteSpace(txtClassification.Text) || string.IsNullOrWhiteSpace(txtDosage.Text) ||
                    string.IsNullOrWhiteSpace(txtManufacturer.Text) || string.IsNullOrWhiteSpace(txtCountryOfOrigin.Text))
                {
            MessageBox.Show("Заполните все обязательные поля", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        int dragId;
        if (!int.TryParse(txtDrugId.Text, out dragId))
        {
            MessageBox.Show("Ошибка при добавлении препарата", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        var newDrags = new Drugs
        {
            Id = dragId,
            name = txtNameDrug.Text,
            classification = txtClassification.Text,
            dosage = txtDosage.Text,
            manufacturer = txtManufacturer.Text,
            countryOfOrigin = txtCountryOfOrigin.Text
        };

        var json = JsonConvert.SerializeObject(newDrags);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await client.PostAsync("drugs", content);
        response.EnsureSuccessStatusCode();

        MessageBox.Show("Препарат успешно добавлен", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        btnLoadDrug_Click(sender, e);
    }
    catch (Exception ex)
    {
        MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
    }
}

private async void btnLoadDrug_Click(object sender, RoutedEventArgs e)
{
    try
    {
        var response = await client.GetStringAsync("drugs");
        var drugs = JsonConvert.DeserializeObject<List<Drugs>>(response);
        dgDrugs.ItemsSource = drugs;
    }
    catch (Exception ex)
    {
        MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
    }
}

private async void btnUpdateDrug_Click(object sender, RoutedEventArgs e)
{
    try
    {
        int drugsId;
        if (!int.TryParse(txtDrugId.Text, out drugsId))
        {
            MessageBox.Show("Ошибка при обновлении препарата", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        var updateDrugs = new Drugs
        {
            Id = drugsId,
            name = txtNameDrug.Text,
            classification = txtClassification.Text,
            dosage = txtDosage.Text,
            manufacturer = txtManufacturer.Text,
            countryOfOrigin = txtCountryOfOrigin.Text
        };

        var json = JsonConvert.SerializeObject(updateDrugs);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await client.PutAsync($"drugs/{drugsId}", content);
        response.EnsureSuccessStatusCode();

        MessageBox.Show("Препарат успешно обновлен", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        btnLoadDrug_Click(sender, e);
    }
    catch (Exception ex)
    {
        MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
    }
}

        private async void MenuItemDeleteDrug_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgDrugs.SelectedItem != null)
                {
                    var selectedConsumption = (Drugs)dgDrugs.SelectedItem; 

                    var response = await client.DeleteAsync($"drugs/{selectedConsumption.Id}");
                    response.EnsureSuccessStatusCode();

                    MessageBox.Show("Препарат успешно удален!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    btnLoadDrug_Click(sender, e);


                }
                else
                {
                    MessageBox.Show("Выберите запись для удаления", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtFilterNameDrug_TextChanged(object sender, TextChangedEventArgs e)
{
    ICollectionView collectionView = CollectionViewSource.GetDefaultView(dgDrugs.ItemsSource);

    if (collectionView != null)
    {
        collectionView.Filter = o =>
        {
            if (o is Drugs drugs)
            {
                return drugs.name.ToLower().StartsWith(txtFilterNameDrug.Text, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        };
    }
}

private void txtFilterClassification_TextChanged(object sender, TextChangedEventArgs e)
{
    ICollectionView collectionView = CollectionViewSource.GetDefaultView(dgDrugs.ItemsSource);

    if (collectionView != null)
    {
        collectionView.Filter = o =>
        {
            if (o is Drugs drugs)
            {
                return drugs.classification.ToLower().StartsWith(txtFilterClassification.Text, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        };
    }
}

        private void txtFilterDosage_TextChanged(object sender, TextChangedEventArgs e)
        {
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(dgDrugs.ItemsSource);

            if (collectionView != null)
            {
                collectionView.Filter = o =>
                {
                    if (o is Drugs drugs)
                    {
                        return drugs.dosage.ToLower().StartsWith(txtFilterDosage.Text, StringComparison.OrdinalIgnoreCase);
                    }
                    return false;
                };
            }
        }

        private void txtFilterManufacturer_TextChanged(object sender, TextChangedEventArgs e)
        {
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(dgDrugs.ItemsSource);

            if (collectionView != null)
            {
                collectionView.Filter = o =>
                {
                    if (o is Drugs drugs)
                    {
                        return drugs.manufacturer.ToLower().StartsWith(txtFilterManufacturer.Text, StringComparison.OrdinalIgnoreCase);
                    }
                    return false;
                };
            }
        }

        private void txtFilterCountryOfOrigin_TextChanged(object sender, TextChangedEventArgs e)
        {
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(dgDrugs.ItemsSource);

            if (collectionView != null)
            {
                collectionView.Filter = o =>
                {
                    if (o is Drugs drugs)
                    {
                        return drugs.countryOfOrigin.ToLower().StartsWith(txtFilterCountryOfOrigin.Text, StringComparison.OrdinalIgnoreCase);
                    }
                    return false;
                };
            }
        }

        private void txtSearchInGridDrug_TextChanged(object sender, TextChangedEventArgs e)
{
    TextBox textBox = sender as TextBox;

    if (textBox != null)
    {
        ICollectionView view = CollectionViewSource.GetDefaultView(dgDrugs.ItemsSource);
        if (view != null)
        {
            view.Filter = item =>
            {
                if (item is Drugs itemType)
                {
                    return itemType.name.ToLower().Contains(textBox.Text) ||
                    itemType.classification.ToLower().Contains(textBox.Text) ||
                    itemType.dosage.ToLower().Contains(textBox.Text) ||
                    itemType.manufacturer.ToLower().Contains(textBox.Text) ||
                    itemType.countryOfOrigin.ToLower().Contains(textBox.Text);
                }
                return false;
            };
        }
    }
}

#endregion

        #region INVENTORY
private async void btnAddInventory_Click(object sender, RoutedEventArgs e)
{
    try
    {
        if (string.IsNullOrWhiteSpace(txtIDInventory.Text) || string.IsNullOrWhiteSpace(txtIDDrug.Text) ||
            string.IsNullOrWhiteSpace(txtIDDepartment.Text) || string.IsNullOrWhiteSpace(txtInventoryCheckDate.Text) ||
            string.IsNullOrWhiteSpace(txtRemainingQuantityOfDrugs.Text) || string.IsNullOrWhiteSpace(txtStorageLocation.Text))
        {
            MessageBox.Show("Заполните все обязательные поля", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        int inventoryId;
        if (!int.TryParse(txtIDInventory.Text, out inventoryId))
        {
            MessageBox.Show("Ошибка при добавлении остатка.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        var newInventory = new Inventory
        {
            Id = inventoryId,
            idDrag = Int32.Parse(txtIDDrug.Text),
            idDepartment = Int32.Parse(txtIDDepartment.Text),
            inventoryCheckDate = DateTime.Parse(txtInventoryCheckDate.Text),
            remainingQuantityOfDrugs = txtRemainingQuantityOfDrugs.Text,
            storageLocation = txtStorageLocation.Text
        };

        var json = JsonConvert.SerializeObject(newInventory);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await client.PostAsync("inventory", content);
        response.EnsureSuccessStatusCode();

        MessageBox.Show("Остаток успешно добавлен", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        btnLoadInventory_Click(sender, e);
    }
    catch (Exception ex)
    {
        MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
    }
}

private async void btnLoadInventory_Click(object sender, RoutedEventArgs e)
{
    try
    {
        var response = await client.GetStringAsync("inventory");
        var record = JsonConvert.DeserializeObject<List<Inventory>>(response);
        dgInventory.ItemsSource = record;
    }
    catch (Exception ex)
    {
        MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
    }
}

private async void btnUpdateInventory_Click(object sender, RoutedEventArgs e)
{
    try
    {
        int inventoryId;
        if (!int.TryParse(txtIDInventory.Text, out inventoryId))
        {
            MessageBox.Show("Ошибка при обновлении записи", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        var updatedInventory = new Inventory
        {
            Id = inventoryId,
            idDrag = Int32.Parse(txtIDDrug.Text),
            idDepartment = Int32.Parse(txtIDDepartment.Text),
            inventoryCheckDate = DateTime.Parse(txtInventoryCheckDate.Text),
            remainingQuantityOfDrugs = txtRemainingQuantityOfDrugs.Text,
            storageLocation = txtStorageLocation.Text
        };

        var json = JsonConvert.SerializeObject(updatedInventory);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await client.PutAsync($"inventory/{inventoryId}", content);
        response.EnsureSuccessStatusCode();

        MessageBox.Show("Остаток успешно обновлен", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        btnLoadInventory_Click(sender, e);
    }
    catch (Exception ex)
    {
        MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
    }
}

        private async void MenuItemDeleteInventory_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgInventory.SelectedItem != null)
                {
                    var selectedConsumption = (Inventory)dgInventory.SelectedItem;

                    var response = await client.DeleteAsync($"inventory/{selectedConsumption.Id}");
                    response.EnsureSuccessStatusCode();

                    MessageBox.Show("Остаток успешно удален!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    btnLoadInventory_Click(sender, e);


                }
                else
                {
                    MessageBox.Show("Выберите запись для удаления", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtSearchInGridInventory_TextChanged(object sender, TextChangedEventArgs e)
{
    TextBox textBox = sender as TextBox;

    if (textBox != null)
    {
        ICollectionView view = CollectionViewSource.GetDefaultView(dgInventory.ItemsSource);
        if (view != null)
        {
            view.Filter = item =>
            {
                if (item is Inventory itemType)
                {
                    return itemType.idDrag.ToString().Contains(textBox.Text) ||
                    itemType.idDepartment.ToString().Contains(textBox.Text) ||
                    itemType.inventoryCheckDate.ToString().Contains(textBox.Text) ||
                    itemType.remainingQuantityOfDrugs.ToLower().Contains(textBox.Text) ||
                    itemType.storageLocation.ToLower().Contains(textBox.Text);
                }
                return false;
            };
        }
    }
}

private void txtFilterIDDrug_TextChanged(object sender, TextChangedEventArgs e)
{
    ICollectionView collectionView = CollectionViewSource.GetDefaultView(dgInventory.ItemsSource);

    if (collectionView != null)
    {
        collectionView.Filter = o =>
        {
            if (o is Inventory inventory)
            {
                return inventory.idDrag.ToString().StartsWith(txtFilterIDDrug.Text, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        };
    }
}

private void txtFilterIDDepartment_TextChanged(object sender, TextChangedEventArgs e)
{
    ICollectionView collectionView = CollectionViewSource.GetDefaultView(dgInventory.ItemsSource);

    if (collectionView != null)
    {
        collectionView.Filter = o =>
        {
            if (o is Inventory inventory)
            {
                return inventory.idDepartment.ToString().StartsWith(txtFilterIDDepartment.Text, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        };
    }
}

private void txtFilterInventoryCheckDate_TextChanged(object sender, TextChangedEventArgs e)
{
    ICollectionView collectionView = CollectionViewSource.GetDefaultView(dgInventory.ItemsSource);

    if (collectionView != null)
    {
        collectionView.Filter = o =>
        {
            if (o is Inventory inventory)
            {
                return inventory.inventoryCheckDate.ToString().StartsWith(txtFilterInventoryCheckDate.Text, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        };
    }
}

private void txtFilterRemainingQuantityOfDrugs_TextChanged(object sender, TextChangedEventArgs e)
{
    ICollectionView collectionView = CollectionViewSource.GetDefaultView(dgInventory.ItemsSource);

    if (collectionView != null)
    {
        collectionView.Filter = o =>
        {
            if (o is Inventory record)
            {
                return record.remainingQuantityOfDrugs.ToLower().StartsWith(txtFilterRemainingQuantityOfDrugs.Text, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        };
    }
}

private void txtFilterStorageLocation_TextChanged(object sender, TextChangedEventArgs e)
{
    ICollectionView collectionView = CollectionViewSource.GetDefaultView(dgInventory.ItemsSource);

    if (collectionView != null)
    {
        collectionView.Filter = o =>
        {
            if (o is Inventory record)
            {
                return record.storageLocation.ToLower().StartsWith(txtFilterStorageLocation.Text, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        };
    }
}
        #endregion

        #region SUPPLIERS
        private async void btnAddSuppliers_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtSupplierId.Text) || string.IsNullOrWhiteSpace(txtCompanyName.Text) ||
                    string.IsNullOrWhiteSpace(txtContactInformation.Text) || string.IsNullOrWhiteSpace(txtSupplyCountry.Text))
                {
                    MessageBox.Show("Заполните все обязательные поля", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                int suppliersId;
                if (!int.TryParse(txtSupplierId.Text, out suppliersId))
                {
                    MessageBox.Show("Ошибка при удалении поставщика", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var newSuppliers = new Suppliers
                {
                    Id = suppliersId,
                    companyName = txtCompanyName.Text,
                    contactInfo = txtContactInformation.Text,
                    supplyCountry = txtSupplyCountry.Text
                };

                var json = JsonConvert.SerializeObject(newSuppliers);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("suppliers", content);
                response.EnsureSuccessStatusCode();

                MessageBox.Show("Поставщик успешно добавлен", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                btnLoadSuppliers_Click(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnLoadSuppliers_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var response = await client.GetStringAsync("suppliers");
                var suppliers = JsonConvert.DeserializeObject<List<Suppliers>>(response);
                dgSuppliers.ItemsSource = suppliers;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnUpdateSuppliers_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int suppliersId;
                if (!int.TryParse(txtSupplierId.Text, out suppliersId))
                {
                    MessageBox.Show("Ошибка при обновлении поставщика", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var updatedPatientCard = new Suppliers
                {
                    Id = suppliersId,
                    companyName = txtCompanyName.Text,
                    contactInfo = txtContactInformation.Text,
                    supplyCountry = txtSupplyCountry.Text
                };

                var json = JsonConvert.SerializeObject(updatedPatientCard);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PutAsync($"suppliers/{suppliersId}", content);
                response.EnsureSuccessStatusCode();

                MessageBox.Show("Поставщик успешно обновлен", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                btnLoadSuppliers_Click(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void MenuItemDeleteSuppliers_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgSuppliers.SelectedItem != null)
                {
                    var selectedConsumption = (Suppliers)dgSuppliers.SelectedItem;

                    var response = await client.DeleteAsync($"suppliers/{selectedConsumption.Id}");
                    response.EnsureSuccessStatusCode();

                    MessageBox.Show("Поставщик успешно удален!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    btnLoadSuppliers_Click(sender, e);


                }
                else
                {
                    MessageBox.Show("Выберите запись для удаления", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtSearchInGridSuppliers_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox != null)
            {
                ICollectionView view = CollectionViewSource.GetDefaultView(dgSuppliers.ItemsSource);
                if (view != null)
                {
                    view.Filter = item =>
                    {
                        if (item is Suppliers itemType)
                        {
                            return itemType.companyName.ToLower().Contains(textBox.Text) ||
                            itemType.contactInfo.ToLower().Contains(textBox.Text) ||
                            itemType.supplyCountry.ToLower().Contains(textBox.Text);
                        }
                        return false;
                    };
                }
            }
        }

        private void txtFilterCompanyName_TextChanged(object sender, TextChangedEventArgs e)
        {
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(dgSuppliers.ItemsSource);

            if (collectionView != null)
            {
                collectionView.Filter = o =>
                {
                    if (o is Suppliers suppliers)
                    {
                        return suppliers.companyName.ToLower().StartsWith(txtFilterCompanyName.Text, StringComparison.OrdinalIgnoreCase);
                    }
                    return false;
                };
            }
        }

        private void txtFilterContactInformation_TextChanged(object sender, TextChangedEventArgs e)
        {
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(dgSuppliers.ItemsSource);

            if (collectionView != null)
            {
                collectionView.Filter = o =>
                {
                    if (o is Suppliers suppliers)
                    {
                        return suppliers.contactInfo.ToString().StartsWith(txtFilterContactInformation.Text, StringComparison.OrdinalIgnoreCase);
                    }
                    return false;
                };
            }
        }

        private void txtFilterSupplyCountry_TextChanged(object sender, TextChangedEventArgs e)
        {
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(dgSuppliers.ItemsSource);

            if (collectionView != null)
            {
                collectionView.Filter = o =>
                {
                    if (o is Suppliers suppliers)
                    {
                        return suppliers.supplyCountry.ToLower().StartsWith(txtFilterSupplyCountry.Text, StringComparison.OrdinalIgnoreCase);
                    }
                    return false;
                };
            }
        }

        #endregion

        #region SUPPLY
        private async void btnAddSupply_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtSupplyId.Text) || string.IsNullOrWhiteSpace(txtIDDrugForSupply.Text) ||
                    string.IsNullOrWhiteSpace(txtIDSupplierForSupply.Text) || string.IsNullOrWhiteSpace(txtSupplyDate.Text) ||
                    string.IsNullOrWhiteSpace(txtQuantity.Text) || string.IsNullOrWhiteSpace(txtCost.Text))
                {
                    MessageBox.Show("Заполните все обязательные поля", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                int supplyId;
                if (!int.TryParse(txtSupplyId.Text, out supplyId))
                {
                    MessageBox.Show("Ошибка при удалении поставки", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var newSupply = new Supply
                {
                    Id = supplyId,
                    idDrug = Int32.Parse(txtIDDrugForSupply.Text),
                    idSupplier = Int32.Parse(txtIDSupplierForSupply.Text),
                    supplyDate = DateTime.Parse(txtSupplyDate.Text),
                    quantity = txtQuantity.Text,
                    cost = txtCost.Text
                };

                var json = JsonConvert.SerializeObject(newSupply);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("supply", content);
                response.EnsureSuccessStatusCode();

                MessageBox.Show("Поставка успешно добавлена", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                btnLoadSupply_Click(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnLoadSupply_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var response = await client.GetStringAsync("supply");
                var supply = JsonConvert.DeserializeObject<List<Supply>>(response);
                dgSupply.ItemsSource = supply;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnUpdateSupply_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int supplyId;
                if (!int.TryParse(txtSupplyId.Text, out supplyId))
                {
                    MessageBox.Show("Ошибка при обновлении поставки", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var updateSupply = new Supply
                {
                    Id = supplyId,
                    idDrug = Int32.Parse(txtIDDrugForSupply.Text),
                    idSupplier = Int32.Parse(txtIDSupplierForSupply.Text),
                    supplyDate = DateTime.Parse(txtSupplyDate.Text),
                    quantity = txtQuantity.Text,
                    cost = txtCost.Text
                };

                var json = JsonConvert.SerializeObject(updateSupply);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PutAsync($"supply/{supplyId}", content);
                response.EnsureSuccessStatusCode();

                MessageBox.Show("Поставка обновлена успешна", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                btnLoadSupply_Click(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void MenuItemDeleteSupply_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgSupply.SelectedItem != null)
                {
                    var selectedConsumption = (Supply)dgSupply.SelectedItem;

                    var response = await client.DeleteAsync($"supply/{selectedConsumption.Id}");
                    response.EnsureSuccessStatusCode();

                    MessageBox.Show("Поставка успешно удалена!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    btnLoadSupply_Click(sender, e);


                }
                else
                {
                    MessageBox.Show("Выберите запись для удаления", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtSearchInGridSupply_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox != null)
            {
                ICollectionView view = CollectionViewSource.GetDefaultView(dgSupply.ItemsSource);
                if (view != null)
                {
                    view.Filter = item =>
                    {
                        if (item is Supply itemType)
                        {
                            return itemType.idDrug.ToString().Contains(textBox.Text) ||
                            itemType.idSupplier.ToString().Contains(textBox.Text) ||
                            itemType.supplyDate.ToString().Contains(textBox.Text) ||
                            itemType.quantity.Contains(textBox.Text) ||
                            itemType.cost.Contains(textBox.Text);
                        }
                        return false;
                    };
                }
            }
        }

        private void txtFilterIDDrugForSupply_TextChanged(object sender, TextChangedEventArgs e)
        {
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(dgSupply.ItemsSource);

            if (collectionView != null)
            {
                collectionView.Filter = o =>
                {
                    if (o is Supply services)
                    {
                        return services.idDrug.ToString().StartsWith(txtFilterIDDrugForSupply.Text, StringComparison.OrdinalIgnoreCase);
                    }
                    return false;
                };
            }
        }

        private void txtFilterIDSupplierForSupply_TextChanged(object sender, TextChangedEventArgs e)
        {
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(dgSupply.ItemsSource);

            if (collectionView != null)
            {
                collectionView.Filter = o =>
                {
                    if (o is Supply services)
                    {
                        return services.idSupplier.ToString().StartsWith(txtFilterIDSupplierForSupply.Text, StringComparison.OrdinalIgnoreCase);
                    }
                    return false;
                };
            }
        }

        private void txtFilterSupplyDate_TextChanged(object sender, TextChangedEventArgs e)
        {
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(dgSupply.ItemsSource);

            if (collectionView != null)
            {
                collectionView.Filter = o =>
                {
                    if (o is Supply services)
                    {
                        return services.supplyDate.ToString().StartsWith(txtFilterSupplyDate.Text, StringComparison.OrdinalIgnoreCase);
                    }
                    return false;
                };
            }
        }
        private void txtFilterQuantity_TextChanged(object sender, TextChangedEventArgs e)
        {
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(dgSupply.ItemsSource);

            if (collectionView != null)
            {
                collectionView.Filter = o =>
                {
                    if (o is Supply services)
                    {
                        return services.quantity.StartsWith(txtFilterQuantity.Text, StringComparison.OrdinalIgnoreCase);
                    }
                    return false;
                };
            }
        }
        private void txtFilterCost_TextChanged(object sender, TextChangedEventArgs e)
        {
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(dgSupply.ItemsSource);

            if (collectionView != null)
            {
                collectionView.Filter = o =>
                {
                    if (o is Supply services)
                    {
                        return services.cost.StartsWith(txtFilterCost.Text, StringComparison.OrdinalIgnoreCase);
                    }
                    return false;
                };
            }
        }

        #endregion

        #region USERS
        private async void btnAddUser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtIDUser.Text) || string.IsNullOrWhiteSpace(txtFullName.Text) ||
                    (string.IsNullOrWhiteSpace(txtPosition.Text) || string.IsNullOrWhiteSpace(txtAccessRight.Text)))
                {
                    MessageBox.Show("Заполните все обязательные поля", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                int userId;
                if (!int.TryParse(txtIDUser.Text, out userId))
                {
                    MessageBox.Show("Ошибка при добавлении пользователя", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var newUser = new Users
                {
                    Id = userId,
                    fullName = txtFullName.Text,
                    position = txtPosition.Text,
                    accessRights = txtAccessRight.Text
                };

                var json = JsonConvert.SerializeObject(newUser);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("users", content);
                response.EnsureSuccessStatusCode();

                MessageBox.Show("Пользователь успешно добавлен", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                btnLoadUser_Click(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnLoadUser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var response = await client.GetStringAsync("users");
                var user = JsonConvert.DeserializeObject<List<Users>>(response);
                dgUsers.ItemsSource = user;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnUpdateUser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int userId;
                if (!int.TryParse(txtIDUser.Text, out userId))
                {
                    MessageBox.Show("Ошибка при обновлении пользователя", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var updatedUser = new Users
                {
                    Id = userId,
                    fullName = txtFullName.Text,
                    position = txtPosition.Text,
                    accessRights = txtAccessRight.Text
                };

                var json = JsonConvert.SerializeObject(updatedUser);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PutAsync($"users/{userId}", content);
                response.EnsureSuccessStatusCode();

                MessageBox.Show("Пользователь обновлен успешно!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                btnLoadUser_Click(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void MenuItemDeleteUsers_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgUsers.SelectedItem != null)
                {
                    var selectedConsumption = (Users)dgUsers.SelectedItem;

                    var response = await client.DeleteAsync($"users/{selectedConsumption.Id}");
                    response.EnsureSuccessStatusCode();

                    MessageBox.Show("Пользователь успешно удален!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    btnLoadUser_Click(sender, e);


                }
                else
                {
                    MessageBox.Show("Выберите запись для удаления", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtFilterFullName_TextChanged(object sender, TextChangedEventArgs e)
        {
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(dgUsers.ItemsSource);
            if (collectionView != null)
            {
                collectionView.Filter = o =>
                {
                    if (o is Users list)
                    {
                        return list.fullName.ToLower().StartsWith(txtFilterFullName.Text, StringComparison.OrdinalIgnoreCase);
                    }
                    return false;
                };
            }
        }

        private void txtFilterPosition_TextChanged(object sender, TextChangedEventArgs e)
        {
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(dgUsers.ItemsSource);
            if (collectionView != null)
            {
                collectionView.Filter = o =>
                {
                    if (o is Users list)
                    {
                        return list.position.ToLower().StartsWith(txtFilterPosition.Text, StringComparison.OrdinalIgnoreCase);
                    }
                    return false;
                };
            }
        }

        private void txtFilterAccessRight_TextChanged(object sender, TextChangedEventArgs e)
        {
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(dgUsers.ItemsSource);
            if (collectionView != null)
            {
                collectionView.Filter = o =>
                {
                    if (o is Users list)
                    {
                        return list.accessRights.ToLower().StartsWith(txtFilterAccessRight.Text, StringComparison.OrdinalIgnoreCase);
                    }
                    return false;
                };
            }
        }

        private void txtSearchInGridUser_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox != null)
            {
                ICollectionView view = CollectionViewSource.GetDefaultView(dgUsers.ItemsSource);
                if (view != null)
                {
                    view.Filter = item =>
                    {
                        if (item is Users itemType)
                        {
                            return itemType.fullName.ToLower().Contains(textBox.Text) ||
                            itemType.position.ToLower().Contains(textBox.Text) ||
                            itemType.accessRights.ToLower().Contains(textBox.Text);
                        }
                        return false;
                    };
                }
            }
        }

        #endregion

        private void txtConsumptionId_TextChanged(object sender, TextChangedEventArgs e)
        {

        }       
    }

}

