async function solicitarTurno() {
    let dni = $("#dni-cliente").val();
    if (!dni) return Swal.fire('Atención', 'Ingrese su número de documento', 'warning');

    Swal.showLoading();
    // 1. Buscamos si el usuario existe
    $.get(`/Asesor/BuscarPorDni?dni=${dni}`, function (res) {
        if (res.status) {
            // Usuario existe, preguntamos prioridad (en un kiosco real esto sería botones)
            Swal.showLoading();
            mostrarMenuPrioridad(res.data.id, res.data.name);
        } else {
            // Usuario no existe, lanzamos registro rápido
            Swal.showLoading();
            registrarYAsignar(dni);
        }
    });
}


// ... aquí van las funciones registrarYAsignar y mostrarMenuPrioridad que te pasé antes ...
async function registrarYAsignar(dni) {
    const { value: formValues } = await Swal.fire({
        title: '¡Bienvenido! Registre sus datos',
        html:
            '<input id="n-name" class="swal2-input" placeholder="Nombre">' +
            '<input id="n-last" class="swal2-input" placeholder="Apellido">' +
            '<input id="n-email" class="swal2-input" placeholder="Correo">',
        confirmButtonText: 'Registrar y Continuar',
        preConfirm: () => {
            return {
                dni: dni,
                name: document.getElementById('n-name').value,
                lastName: document.getElementById('n-last').value,
                email: document.getElementById('n-email').value
            }
        }
    });

    if (formValues) {
        $.post('/Asesor/RegistrarUsuario', formValues, function (res) {
            if (res.status) {
                mostrarMenuPrioridad(res.data.id, res.data.name);
            }
        });
    }
}

async function mostrarMenuPrioridad(userId, nombre) {
    const { value: priorityId } = await Swal.fire({
        title: `Hola ${nombre}, selecciona tu tipo de atención`,
        input: 'radio',
        inputOptions: { '1': 'Normal', '2': 'Prioritario', '3': 'VIP' },
        inputValue: '1',
        confirmButtonText: 'Generar Ticket',
        inputValidator: (value) => { if (!value) return 'Debes elegir una opción'; }
    });
    Swal.showLoading();
    if (priorityId) {
        $.post('/Asesor/AsignarTurno', { userId: userId, priorityId: priorityId }, function (res) {
            // Importante: validar con 'res.success' o 'res.status' según tu controlador
            if (res.success || res.status) {
                Swal.fire({
                    title: '¡TICKET GENERADO!',
                    html: `<h1 class="display-1 fw-bold text-primary">${res.data.ticket}</h1><p>Espere su llamado en la sala.</p>`,
                    icon: 'success',
                    timer: 5000,
                    showConfirmButton: false
                });
                $("#dni-cliente").val("");
            } else {
                // AQUÍ MOSTRARÁ EL MENSAJE: "El usuario ya tiene un turno en proceso"
                Swal.fire({
                    title: 'Atención',
                    text: res.message, // Este es el mensaje que viene del ServiceResponse.Error
                    icon: 'warning',
                    confirmButtonColor: '#3085d6'
                });
            }
        });
    }
}
