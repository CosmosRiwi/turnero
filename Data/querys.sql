DROP DATABASE IF EXISTS sistema_turnos;
CREATE DATABASE sistema_turnos;
USE sistema_turnos;

-- Tabla Madre: Estados de Turno
CREATE TABLE turn_status (
                             id INT PRIMARY KEY AUTO_INCREMENT,
                             name VARCHAR(50) NOT NULL
);

-- Tabla Madre: Prioridades
CREATE TABLE priorities (
                            id INT PRIMARY KEY AUTO_INCREMENT,
                            name VARCHAR(50) NOT NULL
);

-- Tabla de Usuarios (Clientes)
CREATE TABLE users (
                       id INT PRIMARY KEY AUTO_INCREMENT,
                       dni VARCHAR(20) UNIQUE NOT NULL,
                       name VARCHAR(50) NOT NULL,
                       lastName VARCHAR(50) NOT NULL,
                       email VARCHAR(100) UNIQUE NOT NULL,
                       status ENUM('Activo', 'Inactivo') DEFAULT 'Activo' NOT NULL,
                       createdAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                       updatedAt DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);

-- Tabla de Personal (Asesores)
CREATE TABLE staff (
                       id INT PRIMARY KEY AUTO_INCREMENT,
                       username VARCHAR(20) NOT NULL,
                       password TEXT NOT NULL,
                       role ENUM('Asesor', 'Admin') DEFAULT 'Asesor',
                       createdAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                       updatedAt DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);

-- Tabla Principal: Turnos
CREATE TABLE turns (
                       id INT PRIMARY KEY AUTO_INCREMENT,
                       ticket VARCHAR(20) UNIQUE NOT NULL,
                       user_id INT NOT NULL,
                       staff_id INT NULL,
                       status_id INT NOT NULL,
                       priority_id INT NOT NULL,
                       createdAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                       updatedAt DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
                       CONSTRAINT fk_user_turn FOREIGN KEY (user_id) REFERENCES users (id) ON DELETE RESTRICT,
                       CONSTRAINT fk_staff_turn FOREIGN KEY (staff_id) REFERENCES staff (id) ON DELETE RESTRICT,
                       CONSTRAINT fk_status_turn FOREIGN KEY (status_id) REFERENCES turn_status(id) ON DELETE RESTRICT,
                       CONSTRAINT fk_priority_turn FOREIGN KEY (priority_id) REFERENCES priorities(id) ON DELETE RESTRICT
);

-- Historial de comentarios
CREATE TABLE turn_history (
                              id INT PRIMARY KEY AUTO_INCREMENT,
                              turn_id INT NOT NULL,
                              comment TEXT,
                              createdAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                              updatedAt DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
                              CONSTRAINT fk_turn_history FOREIGN KEY (turn_id) REFERENCES turns (id) ON DELETE RESTRICT
);

-- Inserción de datos maestros iniciales
INSERT INTO turn_status (name) VALUES ('Pendiente'), ('En Espera'), ('En Atencion'), ('Finalizado'), ('Cancelado');
INSERT INTO priorities (name) VALUES ('Normal'), ('Prioritario'), ('VIP');

-- Comando para ejecutar la migración del database firts
dotnet ef dbcontext scaffold "Server=157.180.40.190;Database=sistema_turnos;Uid=root;Pwd=CosmosGalaxy;" Pomelo.EntityFrameworkCore.MySql -o Models --context MysqlDbContext --context-dir Data --force
       
       sudo chown $USER:$USER /dev/usb/lp0
       sudo chmod 666 /dev/usb/lp0