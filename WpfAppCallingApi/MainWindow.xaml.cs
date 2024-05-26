using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using WpfAppCallingApi.Models;

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

        #region PATIENTS
        private void btnLoadPatients_Click(object sender, RoutedEventArgs e)
        {
            this.GetPatients();
        }

        private async void GetPatients() 
        {
            try 
            {
                var response = await client.GetStringAsync("patients");
                var patients = JsonConvert.DeserializeObject<List<Patients>>(response);
                dgPatient.ItemsSource = patients;
            }
            catch (Exception ex) 
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void dgPatient_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private async void btnUpdatePatietn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int patientId;
                if (!int.TryParse(txtPatientId.Text, out patientId))
                {
                    MessageBox.Show("Ошибка при обновлении пациента", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var updatedPatient = new Patients
                {
                    Id = patientId,
                    fullName = txtName.Text,
                    dateBirth = DateTime.Parse(txtDateBirth.Text),
                    phoneNumber = txtPhoneNumber.Text,
                    email = txtEmail.Text,
                    address = txtAddress.Text
                };

                var json = JsonConvert.SerializeObject(updatedPatient);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PutAsync($"patients/{patientId}", content);
                response.EnsureSuccessStatusCode();

                MessageBox.Show("Пациент обновлен успешно!.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                btnLoadPatients_Click(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnDeletePatient_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int patientIdToDelete;
                if (int.TryParse(txtPatientIdToDelete.Text, out patientIdToDelete))
                {
                    var response = await client.DeleteAsync($"patients/{patientIdToDelete}");
                    response.EnsureSuccessStatusCode();
                    txtPatientIdToDelete.Clear();
                    MessageBox.Show("Пациент успешно удален!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    btnLoadPatients_Click(sender, e);
                }
                else
                {
                    MessageBox.Show("Ошибка при удалении пациента", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnAddPatient_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtName.Text) || string.IsNullOrWhiteSpace(txtDateBirth.Text) ||
                    string.IsNullOrWhiteSpace(txtPhoneNumber.Text) || string.IsNullOrWhiteSpace(txtEmail.Text) ||
                    string.IsNullOrWhiteSpace(txtAddress.Text))
                {
                    MessageBox.Show("Заполните все обязательные поля", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                int patientId;
                if (!int.TryParse(txtPatientId.Text, out patientId))
                {
                    MessageBox.Show("Ошибка при добавлении пациента", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var newPatient = new Patients
                {
                    Id = patientId,
                    fullName = txtName.Text,
                    dateBirth = DateTime.Parse(txtDateBirth.Text),
                    phoneNumber = txtPhoneNumber.Text,
                    email = txtEmail.Text,
                    address = txtAddress.Text
                };

                if (IsPatientPhoneNumberExists(txtPhoneNumber.Text)) 
                {
                    MessageBox.Show("Пациент с таким же номером телефона уже существует", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (IsPatientIdExists(patientId)) 
                {
                    MessageBox.Show("Пациент с таким же идентификатором уже существует", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!ValidatePhoneNumber(txtPhoneNumber.Text))
                {
                    MessageBox.Show("Неверный формат номера телефона", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var json = JsonConvert.SerializeObject(newPatient);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("patients", content);
                response.EnsureSuccessStatusCode();

                MessageBox.Show("Пациент успешно добавлен", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                btnLoadPatients_Click(sender, e);



            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public bool ValidatePhoneNumber(string phoneNumber)
        {
            // Паттерн для проверки формата номера телефона (примерный формат)
            string phonePattern = @"^\+\d{1,3}\s?\(\d{2,3}\)\s?\d{3}-\d{2}-\d{2}$";

            return Regex.IsMatch(phoneNumber, phonePattern);
        }

        private bool IsPatientPhoneNumberExists(string phoneNumber)
        {
            var existingPatients = (List<Patients>)dgPatient.ItemsSource;
            return existingPatients.Any(p => p.phoneNumber == phoneNumber);
        }

        private bool IsPatientIdExists(int parientId)
        {
            var existingPatients = (List<Patients>)dgPatient.ItemsSource;
            return existingPatients.Any(p => p.Id == parientId);
        }

        private void txtFilterFullName_TextChanged(object sender, TextChangedEventArgs e)
        {
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(dgPatient.ItemsSource);
            if (collectionView != null)
            {
                collectionView.Filter = o =>
                {
                    if (o is Patients patient)
                    {
                        return patient.fullName.StartsWith(txtFilterFullName.Text, StringComparison.OrdinalIgnoreCase);
                    }
                    return false;
                };
            }
        }

        private void txtFilterDateBirth_TextChanged(object sender, TextChangedEventArgs e)
        {
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(dgPatient.ItemsSource);
            if (collectionView != null)
            {
                collectionView.Filter = o =>
                {
                    if (o is Patients patient)
                    {
                        return patient.dateBirth.ToString().StartsWith(txtFilterDateBirth.Text, StringComparison.OrdinalIgnoreCase);
                    }
                    return false;
                };
            }
        }

        private void txtFilterPhoneNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(dgPatient.ItemsSource);
            if (collectionView != null)
            {
                collectionView.Filter = o =>
                {
                    if (o is Patients patient)
                    {
                        return patient.phoneNumber.StartsWith(txtFilterPhoneNumber.Text, StringComparison.OrdinalIgnoreCase);
                    }
                    return false;
                };
            }
        }

        private void txtFilterEmail_TextChanged(object sender, TextChangedEventArgs e)
        {
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(dgPatient.ItemsSource);

            if (collectionView != null)
            {
                collectionView.Filter = o =>
                {
                    if (o is Patients patient)
                    {
                        return patient.email.StartsWith(txtFilterEmail.Text, StringComparison.OrdinalIgnoreCase);
                    }
                    return false;
                };
            }
        }

        private void txtFilterAddress_TextChanged(object sender, TextChangedEventArgs e)
        {
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(dgPatient.ItemsSource);

            if (collectionView != null)
            {
                collectionView.Filter = o =>
                {
                    if (o is Patients patient)
                    {
                        return patient.address.StartsWith(txtFilterAddress.Text, StringComparison.OrdinalIgnoreCase);
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
                ICollectionView view = CollectionViewSource.GetDefaultView(dgPatient.ItemsSource);
                if (view != null)
                {
                    view.Filter = item =>
                    {
                        if (item is Patients itemType)
                        {
                            return itemType.fullName.ToLower().Contains(textBox.Text) ||
                            itemType.dateBirth.ToString().Contains(textBox.Text) ||
                            itemType.phoneNumber.ToLower().Contains(textBox.Text) ||
                            itemType.email.ToLower().Contains(textBox.Text) ||
                            itemType.address.ToLower().Contains(textBox.Text);
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

        #region DOCTORS


        private async void btnAddDoctors_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtNameDoctor.Text) || string.IsNullOrWhiteSpace(txtIDSpecialty.Text) ||
                    string.IsNullOrWhiteSpace(txtContactInfo.Text) || string.IsNullOrWhiteSpace(txtMedicalExperience.Text) ||
                    string.IsNullOrWhiteSpace(txtWorkSchedule.Text))
                {
                    MessageBox.Show("Заполните все обязательные поля", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                int doctorId;
                if (!int.TryParse(txtDoctorId.Text, out doctorId))
                {
                    MessageBox.Show("Ошибка при добавлении доктора", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var newDoctor = new Doctors
                {
                    Id = doctorId,
                    fullName = txtNameDoctor.Text,
                    idSpecialty = Int32.Parse(txtIDSpecialty.Text),
                    contactInfo = txtContactInfo.Text,
                    medicalExperience = txtMedicalExperience.Text,
                    workSchedule = txtWorkSchedule.Text
                };

                if (!ValidateEmail(txtContactInfo.Text))
                {
                    MessageBox.Show("Неверный формат почты", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var json = JsonConvert.SerializeObject(newDoctor);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("doctors", content);
                response.EnsureSuccessStatusCode();

                MessageBox.Show("Доктор успешно добавлен", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                btnLoadDoctors_Click(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public bool ValidateEmail(string email)
        {
            // Паттерн для проверки формата электронной почты
            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

            return Regex.IsMatch(email, emailPattern);
        }

        private async void btnLoadDoctors_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var response = await client.GetStringAsync("doctors");
                var doctors = JsonConvert.DeserializeObject<List<Doctors>>(response);
                dgDoctor.ItemsSource = doctors;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnUpdateDoctors_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int doctorId;
                if (!int.TryParse(txtDoctorId.Text, out doctorId))
                {
                    MessageBox.Show("Ошибка при обновлении доктора", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var updatedDoctor = new Doctors
                {
                    Id = doctorId,
                    fullName = txtNameDoctor.Text,
                    idSpecialty = Int32.Parse(txtIDSpecialty.Text),
                    contactInfo = txtContactInfo.Text,
                    medicalExperience = txtMedicalExperience.Text,
                    workSchedule = txtWorkSchedule.Text
                };

                var json = JsonConvert.SerializeObject(updatedDoctor);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PutAsync($"doctors/{doctorId}", content);
                response.EnsureSuccessStatusCode();

                MessageBox.Show("Доктор успешно обновлен", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                btnLoadDoctors_Click(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnDeleteDoctor_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int doctorIdToDelete;
                if (int.TryParse(txtDoctorIdToDelete.Text, out doctorIdToDelete))
                {
                    var response = await client.DeleteAsync($"doctors/{doctorIdToDelete}");
                    response.EnsureSuccessStatusCode();
                    txtDoctorIdToDelete.Clear();
                    MessageBox.Show("Доктор успешно удален", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    btnLoadDoctors_Click(sender, e);
                }
                else
                {
                    MessageBox.Show("Ошибка при удалении доктора", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtFilterFullNameDoctor_TextChanged(object sender, TextChangedEventArgs e)
        {
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(dgDoctor.ItemsSource);

            if (collectionView != null) 
            {
                collectionView.Filter = o =>
                {
                    if (o is Doctors doctors)
                    {
                        return doctors.fullName.StartsWith(txtFilterFullNameDoctor.Text, StringComparison.OrdinalIgnoreCase);
                    }
                    return false;
                };
            }
        }

        private void txtFilterIDSpecialty_TextChanged(object sender, TextChangedEventArgs e)
        {
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(dgDoctor.ItemsSource);

            if (collectionView != null)
            {
                collectionView.Filter = o =>
                {
                    if (o is Doctors doctors)
                    {
                        return doctors.idSpecialty.ToString().StartsWith(txtFilterIDSpecialty.Text, StringComparison.OrdinalIgnoreCase);
                    }
                    return false;
                };
            }
        }

        private void txtFilterContactInfo_TextChanged(object sender, TextChangedEventArgs e)
        {
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(dgDoctor.ItemsSource);

            if (collectionView != null)
            {
                collectionView.Filter = o =>
                {
                    if (o is Doctors doctors)
                    {
                        return doctors.contactInfo.StartsWith(txtFilterContactInfo.Text, StringComparison.OrdinalIgnoreCase);
                    }
                    return false;
                };
            }
        }

        private void txtFilterMedicalExperience_TextChanged(object sender, TextChangedEventArgs e)
        {
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(dgDoctor.ItemsSource);

            if (collectionView != null)
            {
                collectionView.Filter = o =>
                {
                    if (o is Doctors doctors)
                    {
                        return doctors.medicalExperience.StartsWith(txtFilterMedicalExperience.Text, StringComparison.OrdinalIgnoreCase);
                    }
                    return false;
                };
            }
        }

        private void txtFilterWorkSchedule_TextChanged(object sender, TextChangedEventArgs e)
        {
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(dgDoctor.ItemsSource);

            if (collectionView != null)
            {
                collectionView.Filter = o =>
                {
                    if (o is Doctors doctors)
                    {
                        return doctors.workSchedule.StartsWith(txtFilterWorkSchedule.Text, StringComparison.OrdinalIgnoreCase);
                    }
                    return false;
                };
            }
        }
        private void txtSearchInGridDoctors_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox != null)
            {
                ICollectionView view = CollectionViewSource.GetDefaultView(dgDoctor.ItemsSource);
                if (view != null)
                {
                    view.Filter = item =>
                    {
                        if (item is Doctors itemType)
                        {
                            return itemType.fullName.ToLower().Contains(textBox.Text) ||
                            itemType.idSpecialty.ToString().Contains(textBox.Text) ||
                            itemType.contactInfo.ToLower().Contains(textBox.Text) ||
                            itemType.medicalExperience.ToLower().Contains(textBox.Text) ||
                            itemType.workSchedule.ToLower().Contains(textBox.Text);
                        }
                        return false;
                    };
                }
            }
        }

        #endregion

        #region DESTINATION
        private async void btnAddDestination_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                if (string.IsNullOrWhiteSpace(txtDataDestination.Text) || string.IsNullOrWhiteSpace(txtDescriptionDestination.Text))
                {
                    MessageBox.Show("Заполните все обязательные поля", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                int destinationId;
                if (!int.TryParse(txtDestinationId.Text, out destinationId))
                {
                    MessageBox.Show("Ошибка при добавлении назначения", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var newDestination = new Destination
                {
                    Id = destinationId,
                    dataDestination = DateTime.Parse(txtDataDestination.Text),
                    descriptionDestination = txtDescriptionDestination.Text
                };

                var json = JsonConvert.SerializeObject(newDestination);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("destination", content);
                response.EnsureSuccessStatusCode();

                MessageBox.Show("Назначение успешно добавлено", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                btnLoadDestination_Click(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnLoadDestination_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var response = await client.GetStringAsync("destination");
                var destination = JsonConvert.DeserializeObject<List<Destination>>(response);
                dgDestination.ItemsSource = destination;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnUpdateDestination_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int destinationId;
                if (!int.TryParse(txtDestinationId.Text, out destinationId))
                {
                    MessageBox.Show("Ошибка при обновлении назначения", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var updateDestination = new Destination
                {
                    Id = destinationId,
                    dataDestination = DateTime.Parse(txtDataDestination.Text),
                    descriptionDestination = txtDescriptionDestination.Text
                };

                var json = JsonConvert.SerializeObject(updateDestination);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PutAsync($"destination/{destinationId}", content);
                response.EnsureSuccessStatusCode();

                MessageBox.Show("Назначение успешно обновлено", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                btnLoadDestination_Click(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnDeleteDestination_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int destinationIdToDelete;
                if (int.TryParse(txtDestinationIdToDelete.Text, out destinationIdToDelete))
                {
                    var response = await client.DeleteAsync($"destination/{destinationIdToDelete}");
                    response.EnsureSuccessStatusCode();
                    txtDestinationIdToDelete.Clear();
                    MessageBox.Show("Назначение успешно удалено", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    btnLoadDestination_Click(sender, e);
                }
                else
                {
                    MessageBox.Show("Ошибка при удалении назначения", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtFilterDataDestination_TextChanged(object sender, TextChangedEventArgs e)
        {
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(dgDestination.ItemsSource);

            if (collectionView != null)
            {
                collectionView.Filter = o =>
                {
                    if (o is Destination destination)
                    {
                        return destination.dataDestination.ToString().StartsWith(txtFilterDataDestination.Text, StringComparison.OrdinalIgnoreCase);
                    }
                    return false;
                };
            }
        }

        private void txtFilterDescriptionDestination_TextChanged(object sender, TextChangedEventArgs e)
        {
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(dgDestination.ItemsSource);

            if (collectionView != null)
            {
                collectionView.Filter = o =>
                {
                    if (o is Destination destination)
                    {
                        return destination.descriptionDestination.StartsWith(txtFilterDescriptionDestination.Text, StringComparison.OrdinalIgnoreCase);
                    }
                    return false;
                };
            }
        }

        private void txtSearchInGridDestination_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox != null)
            {
                ICollectionView view = CollectionViewSource.GetDefaultView(dgDestination.ItemsSource);
                if (view != null)
                {
                    view.Filter = item =>
                    {
                        if (item is Destination itemType)
                        {
                            return itemType.dataDestination.ToString().Contains(textBox.Text) ||
                            itemType.descriptionDestination.ToLower().Contains(textBox.Text);
                        }
                        return false;
                    };
                }
            }
        }

        #endregion

        #region RECORD_AND_RECEIVING
        private async void btnAddRecord_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtIDDestination.Text) || string.IsNullOrWhiteSpace(txtIDServices.Text) ||
                    string.IsNullOrWhiteSpace(txtDataAndTimeRecord.Text) || string.IsNullOrWhiteSpace(txtIDDoctor.Text) ||
                    string.IsNullOrWhiteSpace(txtIDPatient.Text) || string.IsNullOrWhiteSpace(txtIDReceivingType.Text))
                {
                    MessageBox.Show("Заполните все обязательные поля", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                int recordId;
                if (!int.TryParse(txtIDRecord.Text, out recordId))
                {
                    MessageBox.Show("Ошибка при добавлении записи.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var newRecord = new Records_on_receiving
                {
                    Id = recordId,
                    idDestination = Int32.Parse(txtIDDestination.Text),
                    idServices = Int32.Parse(txtIDServices.Text),
                    dataAndTimeRecord = DateTime.Parse(txtDataAndTimeRecord.Text),
                    idDoctor = Int32.Parse(txtIDDoctor.Text),
                    idPatient = Int32.Parse(txtIDPatient.Text),
                    IdReceivingType = Int32.Parse(txtIDReceivingType.Text)
                };

                var json = JsonConvert.SerializeObject(newRecord);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("recordonreceiv", content);
                response.EnsureSuccessStatusCode();

                MessageBox.Show("Запись успешно добавлена", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                btnLoadRecord_Click(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnLoadRecord_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var response = await client.GetStringAsync("recordonreceiv");
                var record = JsonConvert.DeserializeObject<List<Records_on_receiving>>(response);
                dgRecord.ItemsSource = record;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnUpdateRecord_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int recordId;
                if (!int.TryParse(txtIDRecord.Text, out recordId))
                {
                    MessageBox.Show("Ошибка при обновлении записи", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var updatedRecord = new Records_on_receiving
                {
                    Id = recordId,
                    idDestination = Int32.Parse(txtIDDestination.Text),
                    idServices = Int32.Parse(txtIDServices.Text),
                    dataAndTimeRecord = DateTime.Parse(txtDataAndTimeRecord.Text),
                    idDoctor = Int32.Parse(txtIDDoctor.Text),
                    idPatient = Int32.Parse(txtIDPatient.Text),
                    IdReceivingType = Int32.Parse(txtIDReceivingType.Text)
                };

                var json = JsonConvert.SerializeObject(updatedRecord);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PutAsync($"recordonreceiv/{recordId}", content);
                response.EnsureSuccessStatusCode();

                MessageBox.Show("Запись успешно обновлена", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                btnLoadRecord_Click(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnDeleteRecord_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int recordrIdToDelete;
                if (int.TryParse(txtRecordIdToDelete.Text, out recordrIdToDelete))
                {
                    var response = await client.DeleteAsync($"recordonreceiv/{recordrIdToDelete}");
                    response.EnsureSuccessStatusCode();
                    txtRecordIdToDelete.Clear();
                    MessageBox.Show("Запись успешно удалена", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    btnLoadRecord_Click(sender, e);
                }
                else
                {
                    MessageBox.Show("Ошибка при удалении записи", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtSearchInGridRecord_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox != null)
            {
                ICollectionView view = CollectionViewSource.GetDefaultView(dgRecord.ItemsSource);
                if (view != null)
                {
                    view.Filter = item =>
                    {
                        if (item is Records_on_receiving itemType)
                        {
                            return itemType.idDestination.ToString().Contains(textBox.Text) ||
                            itemType.idServices.ToString().Contains(textBox.Text) ||
                            itemType.dataAndTimeRecord.ToString().Contains(textBox.Text) ||
                            itemType.idDoctor.ToString().Contains(textBox.Text) ||
                            itemType.idPatient.ToString().Contains(textBox.Text) ||
                            itemType.IdReceivingType.ToString().Contains(textBox.Text);
                        }
                        return false;
                    };
                }
            }
        }

        private void txtFilterIDDestination_TextChanged(object sender, TextChangedEventArgs e)
        {
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(dgRecord.ItemsSource);

            if (collectionView != null)
            {
                collectionView.Filter = o =>
                {
                    if (o is Records_on_receiving record)
                    {
                        return record.idDestination.ToString().StartsWith(txtFilterIDDestination.Text, StringComparison.OrdinalIgnoreCase);
                    }
                    return false;
                };
            }
        }

        private void txtFilterIDServices_TextChanged(object sender, TextChangedEventArgs e)
        {
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(dgRecord.ItemsSource);

            if (collectionView != null)
            {
                collectionView.Filter = o =>
                {
                    if (o is Records_on_receiving record)
                    {
                        return record.idServices.ToString().StartsWith(txtFilterIDServices.Text, StringComparison.OrdinalIgnoreCase);
                    }
                    return false;
                };
            }
        }

        private void txtFilterDataAndTimeRecord_TextChanged(object sender, TextChangedEventArgs e)
        {
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(dgRecord.ItemsSource);

            if (collectionView != null)
            {
                collectionView.Filter = o =>
                {
                    if (o is Records_on_receiving record)
                    {
                        return record.dataAndTimeRecord.ToString().StartsWith(txtFilterDataAndTimeRecord.Text, StringComparison.OrdinalIgnoreCase);
                    }
                    return false;
                };
            }
        }

        private void txtFilterIDDoctor_TextChanged(object sender, TextChangedEventArgs e)
        {
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(dgRecord.ItemsSource);

            if (collectionView != null)
            {
                collectionView.Filter = o =>
                {
                    if (o is Records_on_receiving record)
                    {
                        return record.idDoctor.ToString().StartsWith(txtFilterIDDoctor.Text, StringComparison.OrdinalIgnoreCase);
                    }
                    return false;
                };
            }
        }

        private void txtFilterIDPatient_TextChanged(object sender, TextChangedEventArgs e)
        {
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(dgRecord.ItemsSource);

            if (collectionView != null)
            {
                collectionView.Filter = o =>
                {
                    if (o is Records_on_receiving record)
                    {
                        return record.idPatient.ToString().StartsWith(txtFilterIDPatient.Text, StringComparison.OrdinalIgnoreCase);
                    }
                    return false;
                };
            }
        }

        private void txtFilteIDReceivingType_TextChanged(object sender, TextChangedEventArgs e)
        {
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(dgRecord.ItemsSource);

            if (collectionView != null)
            {
                collectionView.Filter = o =>
                {
                    if (o is Records_on_receiving record)
                    {
                        return record.IdReceivingType.ToString().StartsWith(txtFilterIDReceivingType.Text, StringComparison.OrdinalIgnoreCase);
                    }
                    return false;
                };
            }
        }

        #endregion

        #region PATIENT_CARD
        private async void btnAddPatientCard_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtIdPatient.Text) || string.IsNullOrWhiteSpace(txtIdListOfDiseases.Text) ||
                    string.IsNullOrWhiteSpace(txtDataRecord.Text) || string.IsNullOrWhiteSpace(txtDescriptionPatient.Text) ||
                    string.IsNullOrWhiteSpace(txtProceduresAndTreatments.Text))
                {
                    MessageBox.Show("Заполните все обязательные поля", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                int patientCardId;
                if (!int.TryParse(txtPatientCardId.Text, out patientCardId))
                {
                    MessageBox.Show("Ошибка при удалении карточки пациента", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var newPatientCard = new Patient_card
                {
                    Id = patientCardId,
                    idPatient = Int32.Parse(txtIdPatient.Text),
                    idListOfDiseases = Int32.Parse(txtIdListOfDiseases.Text),
                    dataRecord = DateTime.Parse(txtDataRecord.Text),
                    descriptionPatient = txtDescriptionPatient.Text,
                    proceduresAndTreatments = txtProceduresAndTreatments.Text
                };

                var json = JsonConvert.SerializeObject(newPatientCard);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("patientcard", content);
                response.EnsureSuccessStatusCode();

                MessageBox.Show("Карточка пациента успешно добавлена", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                btnLoadPatientCard_Click(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnLoadPatientCard_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var response = await client.GetStringAsync("patientcard");
                var patientCard = JsonConvert.DeserializeObject<List<Patient_card>>(response);
                dgPatientCard.ItemsSource = patientCard;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnUpdatePatientCard_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int patientCardId;
                if (!int.TryParse(txtPatientCardId.Text, out patientCardId))
                {
                    MessageBox.Show("Ошибка при обновлении карточки пациента", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var updatedPatientCard = new Patient_card
                {
                    Id = patientCardId,
                    idPatient = Int32.Parse(txtIdPatient.Text),
                    idListOfDiseases = Int32.Parse(txtIdListOfDiseases.Text),
                    dataRecord = DateTime.Parse(txtDataRecord.Text),
                    descriptionPatient = txtDescriptionPatient.Text,
                    proceduresAndTreatments = txtProceduresAndTreatments.Text
                };

                var json = JsonConvert.SerializeObject(updatedPatientCard);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PutAsync($"patientcard/{patientCardId}", content);
                response.EnsureSuccessStatusCode();

                MessageBox.Show("Карточка пациента успешно обновлена", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                btnLoadPatientCard_Click(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnDeletePatientCard_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int patientCardIdToDelete;
                if (int.TryParse(txtPatientCardIdToDelete.Text, out patientCardIdToDelete))
                {
                    var response = await client.DeleteAsync($"patientcard/{patientCardIdToDelete}");
                    response.EnsureSuccessStatusCode();
                    txtPatientCardIdToDelete.Clear();
                    MessageBox.Show("Карточка пациента успешно удалена", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    btnLoadPatientCard_Click(sender, e);
                }
                else
                {
                    MessageBox.Show("Ошибка при удалении карточки пациента", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtSearchInGridPatientCard_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox != null)
            {
                ICollectionView view = CollectionViewSource.GetDefaultView(dgPatientCard.ItemsSource);
                if (view != null)
                {
                    view.Filter = item =>
                    {
                        if (item is Patient_card itemType)
                        {
                            return itemType.idPatient.ToString().Contains(textBox.Text) ||
                            itemType.idListOfDiseases.ToString().Contains(textBox.Text) ||
                            itemType.dataRecord.ToString().Contains(textBox.Text) ||
                            itemType.descriptionPatient.ToLower().Contains(textBox.Text) ||
                            itemType.proceduresAndTreatments.ToLower().Contains(textBox.Text);
                        }
                        return false;
                    };
                }
            }
        }

        private void txtFilterIDPatientFromPatientCard_TextChanged(object sender, TextChangedEventArgs e)
        {
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(dgPatientCard.ItemsSource);

            if (collectionView != null)
            {
                collectionView.Filter = o =>
                {
                    if (o is Patient_card patientCard)
                    {
                        return patientCard.idPatient.ToString().StartsWith(txtFilterIdPatient.Text, StringComparison.OrdinalIgnoreCase);
                    }
                    return false;
                };
            }
        }

        private void txtFilterIdListOfDiseases_TextChanged(object sender, TextChangedEventArgs e)
        {
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(dgPatientCard.ItemsSource);

            if (collectionView != null)
            {
                collectionView.Filter = o =>
                {
                    if (o is Patient_card patientCard)
                    {
                        return patientCard.idListOfDiseases.ToString().StartsWith(txtFilterIdListOfDiseases.Text, StringComparison.OrdinalIgnoreCase);
                    }
                    return false;
                };
            }
        }

        private void txtFilterDataRecord_TextChanged(object sender, TextChangedEventArgs e)
        {
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(dgPatientCard.ItemsSource);

            if (collectionView != null)
            {
                collectionView.Filter = o =>
                {
                    if (o is Patient_card patientCard)
                    {
                        return patientCard.dataRecord.ToString().StartsWith(txtFilterDataRecord.Text, StringComparison.OrdinalIgnoreCase);
                    }
                    return false;
                };
            }
        }

        private void txtFilterDescriptionPatients_TextChanged(object sender, TextChangedEventArgs e)
        {
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(dgPatientCard.ItemsSource);

            if (collectionView != null)
            {
                collectionView.Filter = o =>
                {
                    if (o is Patient_card patientCard)
                    {
                        return patientCard.descriptionPatient.ToLower().StartsWith(txtFilterDescriptionPatients.Text, StringComparison.OrdinalIgnoreCase);
                    }
                    return false;
                };
            }
        }

        private void txtFilterProceduresAndTreatments_TextChanged(object sender, TextChangedEventArgs e)
        {
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(dgPatientCard.ItemsSource);

            if (collectionView != null)
            {
                collectionView.Filter = o =>
                {
                    if (o is Patient_card patientCard)
                    {
                        return patientCard.proceduresAndTreatments.ToLower().StartsWith(txtFilterProceduresAndTreatments.Text, StringComparison.OrdinalIgnoreCase);
                    }
                    return false;
                };
            }
        }

        #endregion

        #region SERVICES
        private async void btnAddServices_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtNameServices.Text) || string.IsNullOrWhiteSpace(txtDescriptionServices.Text) ||
                    string.IsNullOrWhiteSpace(txtPrice.Text))
                {
                    MessageBox.Show("Заполните все обязательные поля", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                int servicesId;
                if (!int.TryParse(txtServicesId.Text, out servicesId))
                {
                    MessageBox.Show("Ошибка при удалении услуги", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var newServices = new Services
                {
                    Id = servicesId,
                    nameService = txtNameServices.Text,
                    descriptionService = txtDescriptionServices.Text,
                    price = decimal.Parse(txtPrice.Text)
                };

                var json = JsonConvert.SerializeObject(newServices);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("services", content);
                response.EnsureSuccessStatusCode();

                MessageBox.Show("Услуга успешно добавлена", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                btnLoadServices_Click(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnLoadServices_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var response = await client.GetStringAsync("services");
                var services = JsonConvert.DeserializeObject<List<Services>>(response);
                dgServices.ItemsSource = services;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnUpdateServices_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int servicesId;
                if (!int.TryParse(txtServicesId.Text, out servicesId))
                {
                    MessageBox.Show("Ошибка при обновлении услуги", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var updateServices = new Services
                {
                    Id = servicesId,
                    nameService = txtNameServices.Text,
                    descriptionService = txtDescriptionServices.Text,
                    price = decimal.Parse(txtPrice.Text)
                };

                var json = JsonConvert.SerializeObject(updateServices);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PutAsync($"services/{servicesId}", content);
                response.EnsureSuccessStatusCode();

                MessageBox.Show("Услуга обновлена успешна", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                btnLoadServices_Click(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnDeleteServices_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int servicesIdToDelete;
                if (int.TryParse(txtServicesIdToDelete.Text, out servicesIdToDelete))
                {
                    var response = await client.DeleteAsync($"services/{servicesIdToDelete}");
                    response.EnsureSuccessStatusCode();
                    txtServicesIdToDelete.Clear();
                    MessageBox.Show("Услуга удалена успешно", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    btnLoadServices_Click(sender, e);
                }
                else
                {
                    MessageBox.Show("Ошибка при удалении услуги", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtSearchInGridServices_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox != null)
            {
                ICollectionView view = CollectionViewSource.GetDefaultView(dgServices.ItemsSource);
                if (view != null)
                {
                    view.Filter = item =>
                    {
                        if (item is Services itemType)
                        {
                            return itemType.nameService.ToLower().Contains(textBox.Text) ||
                            itemType.descriptionService.ToLower().Contains(textBox.Text) ||
                            itemType.price.ToString().Contains(textBox.Text);
                        }
                        return false;
                    };
                }
            }
        }

        private void txtFilterNameServices_TextChanged(object sender, TextChangedEventArgs e)
        {
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(dgServices.ItemsSource);

            if (collectionView != null)
            {
                collectionView.Filter = o =>
                {
                    if (o is Services services)
                    {
                        return services.nameService.ToLower().StartsWith(txtFilterNameServices.Text, StringComparison.OrdinalIgnoreCase);
                    }
                    return false;
                };
            }
        }

        private void txtFilterDescriptionServices_TextChanged(object sender, TextChangedEventArgs e)
        {
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(dgServices.ItemsSource);

            if (collectionView != null)
            {
                collectionView.Filter = o =>
                {
                    if (o is Services services)
                    {
                        return services.descriptionService.ToLower().StartsWith(txtFilterDescriptionServices.Text, StringComparison.OrdinalIgnoreCase);
                    }
                    return false;
                };
            }
        }

        private void txtFilterPrice_TextChanged(object sender, TextChangedEventArgs e)
        {
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(dgServices.ItemsSource);

            if (collectionView != null)
            {
                collectionView.Filter = o =>
                {
                    if (o is Services services)
                    {
                        return services.price.ToString().StartsWith(txtFilterPrice.Text, StringComparison.OrdinalIgnoreCase);
                    }
                    return false;
                };
            }
        }

        #endregion

        #region LISTOFDISEASES
        private async void btnAddListOfDiseases_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtDiseases.Text) || string.IsNullOrWhiteSpace(txtTreatment.Text))
                {
                    MessageBox.Show("Заполните все обязательные поля", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                int listId;
                if (!int.TryParse(txtIDListOfDiseases.Text, out listId))
                {
                    MessageBox.Show("Ошибка при добавлении списка", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var newList = new List_of_diseases
                {
                    Id = listId,
                    diseases = txtDiseases.Text,
                    treatment = txtTreatment.Text
                };

                var json = JsonConvert.SerializeObject(newList);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("listdiseases", content);
                response.EnsureSuccessStatusCode();

                MessageBox.Show("Список успешно добавлен", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                btnLoadListOfDiseases_Click(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnLoadListOfDiseases_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var response = await client.GetStringAsync("listdiseases");
                var listOfDiseases = JsonConvert.DeserializeObject<List<List_of_diseases>>(response);
                dgListOfDiseases.ItemsSource = listOfDiseases;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnUpdateListOfDiseases_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int listOfDiseasesId;
                if (!int.TryParse(txtIDListOfDiseases.Text, out listOfDiseasesId))
                {
                    MessageBox.Show("Ошибка при обновлении списка болезней", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var updatedList = new List_of_diseases
                {
                    Id = listOfDiseasesId,
                    diseases = txtDiseases.Text,
                    treatment = txtTreatment.Text
                };

                var json = JsonConvert.SerializeObject(updatedList);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PutAsync($"listdiseases/{listOfDiseasesId}", content);
                response.EnsureSuccessStatusCode();

                MessageBox.Show("Список обновлен успешно!.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                btnLoadListOfDiseases_Click(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnDeleteListOfDiseases_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int listIdToDelete;
                if (int.TryParse(txtListOfDiseasesIdToDelete.Text, out listIdToDelete))
                {
                    var response = await client.DeleteAsync($"listdiseases/{listIdToDelete}");
                    response.EnsureSuccessStatusCode();
                    txtListOfDiseasesIdToDelete.Clear();
                    MessageBox.Show("Список успешно удален!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    btnLoadListOfDiseases_Click(sender, e);
                }
                else
                {
                    MessageBox.Show("Ошибка при удалении списка", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtFilterDiseases_TextChanged(object sender, TextChangedEventArgs e)
        {
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(dgListOfDiseases.ItemsSource);
            if (collectionView != null)
            {
                collectionView.Filter = o =>
                {
                    if (o is List_of_diseases list)
                    {
                        return list.diseases.ToLower().StartsWith(txtFilterDiseases.Text, StringComparison.OrdinalIgnoreCase);
                    }
                    return false;
                };
            }
        }

        private void txtFilterTreatment_TextChanged(object sender, TextChangedEventArgs e)
        {
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(dgListOfDiseases.ItemsSource);
            if (collectionView != null)
            {
                collectionView.Filter = o =>
                {
                    if (o is List_of_diseases list)
                    {
                        return list.treatment.ToLower().StartsWith(txtFilterTreatment.Text, StringComparison.OrdinalIgnoreCase);
                    }
                    return false;
                };
            }
        }

        private void txtSearchInGridListOfDiseases_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox != null)
            {
                ICollectionView view = CollectionViewSource.GetDefaultView(dgListOfDiseases.ItemsSource);
                if (view != null)
                {
                    view.Filter = item =>
                    {
                        if (item is List_of_diseases itemType)
                        {
                            return itemType.diseases.ToLower().Contains(textBox.Text) ||
                            itemType.treatment.ToLower().Contains(textBox.Text);
                            ;
                        }
                        return false;
                    };
                }
            }
        }

        #endregion

        #region RECEIVINGTYPE
        private async void btnAddReceivingType_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtNameReceivingType.Text))
                {
                    MessageBox.Show("Заполните все обязательные поля", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                int typeId;
                if (!int.TryParse(txtIDReceivingtype.Text, out typeId))
                {
                    MessageBox.Show("Ошибка при добавлении типа приема", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var newDoctor = new Receiving_type
                {
                    Id = typeId,
                    nameReceiving = txtNameReceivingType.Text
                };

                var json = JsonConvert.SerializeObject(newDoctor);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("receivingtype", content);
                response.EnsureSuccessStatusCode();

                MessageBox.Show("Тип приема успешно добавлен", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                btnLoadReceivingType_Click(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnLoadReceivingType_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var response = await client.GetStringAsync("receivingtype");
                var type = JsonConvert.DeserializeObject<List<Receiving_type>>(response);
                dgReceivingType.ItemsSource = type;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnUpdateReceivingType_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int typeId;
                if (!int.TryParse(txtIDReceivingtype.Text, out typeId))
                {
                    MessageBox.Show("Ошибка при обновлении типа приема", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var updatedDoctor = new Receiving_type
                {
                    Id = typeId,
                    nameReceiving = txtNameReceivingType.Text
                };

                var json = JsonConvert.SerializeObject(updatedDoctor);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PutAsync($"receivingtype/{typeId}", content);
                response.EnsureSuccessStatusCode();

                MessageBox.Show("Тип приема успешно обновлен", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                btnLoadReceivingType_Click(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnDeleteReceivingType_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int typeIdToDelete;
                if (int.TryParse(txtReceivingTypeIdToDelete.Text, out typeIdToDelete))
                {
                    var response = await client.DeleteAsync($"receivingtype/{typeIdToDelete}");
                    response.EnsureSuccessStatusCode();
                    txtReceivingTypeIdToDelete.Clear();
                    MessageBox.Show("Тип приема успешно удален", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    btnLoadReceivingType_Click(sender, e);
                }
                else
                {
                    MessageBox.Show("Ошибка при удалении типа приема", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtFilterReceivingType_TextChanged(object sender, TextChangedEventArgs e)
        {
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(dgReceivingType.ItemsSource);
            if (collectionView != null)
            {
                collectionView.Filter = o =>
                {
                    if (o is Receiving_type type)
                    {
                        return type.nameReceiving.ToLower().StartsWith(txtFilterNameReceivingType.Text, StringComparison.OrdinalIgnoreCase);
                    }
                    return false;
                };
            }
        }

        private void txtSearchInGridReceivingType_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox != null)
            {
                ICollectionView view = CollectionViewSource.GetDefaultView(dgReceivingType.ItemsSource);
                if (view != null)
                {
                    view.Filter = item =>
                    {
                        if (item is Receiving_type type)
                        {
                            return type.nameReceiving.ToLower().Contains(textBox.Text);
                        }
                        return false;
                    };
                }
            }
        }
        #endregion

        private void TabControl_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

        }

        

        
    }
}
