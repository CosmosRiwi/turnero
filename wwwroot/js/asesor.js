function CallNext() {
    $.post('/Asesor/CallNext', function (response) {
        if (response.success && response.data) {
            // Animación de éxito
            Swal.fire({
                title: '¡Turno Llamado!',
                text: response.message,
                icon: 'success',
                timer: 2000, // Se cierra solo en 2 segundos
                showConfirmButton: false
            });

            $("#hdn-turno-id").val(response.data.id);
            $("#atencion-vacia").hide();
            $("#atencion-activa").fadeIn();

            $("#txt-ticket").text(response.data.ticket);
            $("#txt-cliente").text(response.data.user.name + " " + response.data.user.lastName);
            $("#hdn-turno-id").val(response.data.id);
        } else {
            // Mensaje si no hay más turnos
            Swal.fire({
                title: 'Atención',
                text: response.message,
                icon: 'info',
                confirmButtonColor: '#3085d6'
            });
        }
    });
}

async function confirmFinish() {
    const {value: comentario} = await Swal.fire({
        title: 'Finalizar Atención',
        input: 'textarea',
        inputLabel: 'Escribe un comentario sobre la atención',
        inputPlaceholder: 'El cliente solicitó información sobre...',
        showCancelButton: true,
        confirmButtonText: 'Guardar y Finalizar',
        cancelButtonText: 'Cancelar',
        inputValidator: (value) => {
            if (!value) {
                return '¡Debes escribir un comentario para finalizar!';
            }
        }
    });

    if (comentario) {
        let id = $("#hdn-turno-id").val();

        $.post('/Asesor/CompleteTurn', {turnId: id, comment: comentario}, function (response) {
            if (response.success) {
                Swal.fire('¡Listo!', response.message, 'success');
                $("#atencion-activa").hide();
                $("#atencion-vacia").show();

                cleanAttentionPanel(); // Limpia la pantalla central sin recargar
                updateWaitList(); // Refresca la lista lateral (la función que hicimos para el Polling)
            } else {
                Swal.fire('Error', response.message, 'error');
            }
        });
    }
}


function updateWaitList() {
    $.get('/Asesor/GetWaitingList', function (response) {
        // 1. Actualizamos el contador en el título
        $("#contador-espera").text(response.count);

        // 2. Limpiamos y repintamos la lista
        let listaHtml = "";
        response.items.forEach(turno => {
            listaHtml += `
                <li class="list-group-item d-flex justify-content-between align-items-center animate__animated animate__fadeIn">
                    ${turno.ticket}
                    <span class="badge bg-secondary rounded-pill">${turno.time}</span>
                </li>`;
        });

        $("#lista-espera").html(listaHtml);
    });
}

async function cancelAttention() {
    const {value: motivo} = await Swal.fire({
        title: '¿Cancelar turno?',
        text: "El turno se marcará como cancelado",
        icon: 'warning',
        input: 'text',
        inputPlaceholder: 'Motivo (ej: Cliente no se presentó)',
        showCancelButton: true,
        confirmButtonText: 'Sí, cancelar'
    });

    if (motivo) {
        let id = $("#hdn-turno-id").val();
        $.post('/Asesor/CancelTurn', {turnId: id, reason: motivo}, function (response) {
            Swal.fire('Cancelado', response.message, 'info');
            cleanAttentionPanel(); // Limpia la pantalla central sin recargar
            updateWaitList(); // Refresca la lista lateral (la función que hicimos para el Polling)
        });
    }
}

function cleanAttentionPanel() {
    // 1. Ocultamos el panel de atención activa con una animación suave
    $("#atencion-activa").fadeOut(300, function () {
        // 2. Mostramos el panel vacío
        $("#atencion-vacia").fadeIn();
        // 3. Limpiamos los textos e IDs para que no queden rastros
        $("#txt-ticket").text("---");
        $("#txt-cliente").text("Nombre del Cliente");
        $("#hdn-turno-id").val("");
    });
}

function buscarORegistrar() {
    let dni = $("#dni-busqueda").val();
    if (!dni) return Swal.fire('Error', 'Ingrese un DNI', 'error');

    $.get(`/Asesor/BuscarPorDni?dni=${dni}`, function (response) {
        if (response.status) {
            // USUARIO EXISTE: Preguntar prioridad
            seleccionarPrioridad(response.data.id, response.data.name);
        } else {
            // NO EXISTE: Registrar y luego asignar turno
            registrarNuevoUsuario(dni);
        }
    });
}

async function seleccionarPrioridad(userId, nombre) {
    const {value: priorityId} = await Swal.fire({
        title: `Asignar Turno a ${nombre}`,
        input: 'select',
        inputOptions: {
            '1': 'Normal',
            '2': 'Prioritario',
            '3': 'VIP'
        },
        inputPlaceholder: 'Seleccione la prioridad',
        showCancelButton: true,
        confirmButtonText: 'Generar Ticket',
        inputValidator: (value) => {
            if (!value) return 'Debes seleccionar una prioridad'
        }
    });

    if (priorityId) {
        $.post('/Asesor/AsignarTurno', {userId: userId, priorityId: priorityId}, function (res) {
            if (res.status) {
                Swal.fire('Ticket Generado', `Número: ${res.data.ticket}`, 'success');
                actualizarListaEspera(); // Refrescamos la lista lateral
                // AQUÍ LLAMARÍAS A LA IMPRESORA DE UBUNTU
            }
        });
    }
}

async function registrarNuevoUsuario(dni) {
    const {value: formValues} = await Swal.fire({
        title: 'Registrar Nuevo Cliente',
        html:
            `<input id="swal-name" class="swal2-input" placeholder="Nombres">` +
            `<input id="swal-lastname" class="swal2-input" placeholder="Apellidos">` +
            `<input id="swal-email" class="swal2-input" placeholder="Correo Electrónico">`,
        focusConfirm: false,
        preConfirm: () => {
            return {
                dni: dni,
                name: document.getElementById('swal-name').value,
                lastName: document.getElementById('swal-lastname').value,
                email: document.getElementById('swal-email').value
            }
        }
    });

    if (formValues) {
        $.post('/Asesor/RegistrarUsuario', formValues, function (response) {
            if (response.status) {
                // Dentro del éxito de registrarNuevoUsuario:
                Swal.fire('Registrado', response.message, 'success').then(() => {
                    // Pasamos el ID del usuario recién creado para darle su turno
                    seleccionarPrioridad(response.data.id, response.data.name);
                });
            } else {
                Swal.fire('Error', response.message, 'error');
            }
        });
    }
}

document.addEventListener('DOMContentLoaded', function () {
    // También la ejecutamos una vez al cargar la página
    updateWaitList();
    // Ejecutar cada 10 segundos (10000 milisegundos)
    setInterval(updateWaitList, 10000);
    
    $.get('/Asesor/GetActiveTurn', function(res) {
        // Usamos res.Status porque es lo que devuelve tu ServiceResponse
        if (res.Status) {
            // Ocultamos el mensaje de "No hay nadie" y mostramos el panel activo
            $("#atencion-vacia").hide();
            $("#atencion-activa").show();

            // Llenamos los datos con lo que trajo el servicio
            $("#txt-ticket").text(res.Data.Ticket);
            $("#txt-cliente").text(res.Data.User.Name + " " + res.Data.User.LastName);
            $("#hdn-turno-id").val(res.Data.Id);
            $("#txt-prioridad").text(res.Data.Priority.Name);

            Swal.fire({
                title: 'Sesión Recuperada',
                text: 'Tienes una atención en curso: ' + res.Data.Ticket,
                icon: 'info',
                timer: 2000,
                showConfirmButton: false
            });
        }
    });
});
