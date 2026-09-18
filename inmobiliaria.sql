-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Servidor: 127.0.0.1
-- Versión del servidor: 10.4.32-MariaDB
-- Versión de PHP: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Base de datos: `inmobiliaria`
--

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `inquilino`
--

CREATE TABLE `inquilino` (
  `ID_inquilino` int(11) NOT NULL,
  `nombre` varchar(30) NOT NULL,
  `apellido` varchar(30) NOT NULL,
  `dni` varchar(15) NOT NULL,
  `telefono` varchar(20) NOT NULL,
  `email` varchar(255) NOT NULL,
  `direccion` varchar(255) NOT NULL,
  `estado` tinyint(4) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `propietario`
--

CREATE TABLE `propietario` (
  `ID_propietario` int(11) NOT NULL,
  `nombre` varchar(30) NOT NULL,
  `apellido` varchar(30) NOT NULL,
  `dni` varchar(15) NOT NULL,
  `telefono` varchar(20) NOT NULL,
  `email` varchar(255) NOT NULL,
  `direccion` varchar(255) NOT NULL,
  `estado` tinyint(4) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `tipo_inmueble`
--

CREATE TABLE `tipo_inmueble` (
  `ID_tipo_inmueble` int(11) NOT NULL,
  `nombre` varchar(50) NOT NULL,
  `estado` tinyint(4) NOT NULL DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `inmueble`
--

CREATE TABLE `inmueble` (
  `ID_inmueble` int(11) NOT NULL,
  `direccion` varchar(255) NOT NULL,
  `cupo` int(11) NOT NULL,
  `precio_dia` decimal(10,2) NOT NULL,
  `porcentaje_reserva` decimal(5,2) NOT NULL,
  `latitud` decimal(10,6) NOT NULL,
  `longitud` decimal(10,6) NOT NULL,
  `portada` varchar(255) DEFAULT NULL,
  `disponible` tinyint(4) NOT NULL DEFAULT 1,
  `ID_tipo_inmueble` int(11) NOT NULL,
  `ID_propietario` int(11) NOT NULL,
  `estado` tinyint(4) NOT NULL DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `reserva`
--

CREATE TABLE `reserva` (
  `ID_reserva` int(11) NOT NULL,
  `fecha_ingreso` date NOT NULL,
  `fecha_egreso` date NOT NULL,
  `fecha_cancelacion` date DEFAULT NULL,
  `monto_dia` decimal(10,2) NOT NULL,
  `ID_inmueble` int(11) NOT NULL,
  `ID_inquilino` int(11) NOT NULL,
  `ID_usuario_creador` int(11) NOT NULL,
  `ID_usuario_finalizador` int(11) DEFAULT NULL,
  `estado` tinyint(4) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `imagen`
--

CREATE TABLE `imagen` (
  `ID_imagen` int(11) NOT NULL,
  `url` varchar(255) NOT NULL,
  `ID_inmueble` int(11) NOT NULL,
  `estado` tinyint(4) NOT NULL DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `pago`
--

CREATE TABLE `pago` (
  `ID_pago` int(11) NOT NULL,
  `concepto` varchar(100) NOT NULL,
  `fecha_pago` date NOT NULL,
  `importe` decimal(10,2) NOT NULL,
  `ID_reserva` int(11) NOT NULL,
  `estado` tinyint(4) NOT NULL DEFAULT 1,
  `ID_usuario_creador` int(11) NOT NULL,
  `ID_usuario_anulador` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `usuario`
--

CREATE TABLE `usuario` (
  `ID_usuario` int(11) NOT NULL,
  `email` varchar(255) NOT NULL,
  `password` varchar(255) NOT NULL,
  `avatar` varchar(255) DEFAULT NULL,
  `nombre` varchar(30) NOT NULL,
  `apellido` varchar(30) NOT NULL,
  `dni` varchar(15) NOT NULL,
  `telefono` varchar(20) DEFAULT NULL,
  `direccion` varchar(255) DEFAULT NULL,
  `rol` enum('Empleado','Administrador') NOT NULL,
  `estado` tinyint(4) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `usuario`
--

INSERT INTO `usuario` (`ID_usuario`, `email`, `password`, `avatar`, `nombre`, `apellido`, `dni`, `telefono`, `direccion`, `rol`, `estado`) VALUES
(1, 'admin@inmobiliaria.com', 'AQAAAAIAAYagAAAAEAcBv0jX5BrkWtOBeeckXDRbcnrB36cdsrCTikh9pPVqhv6pGtviwFa9rhcyDfiIlw==', NULL, 'Ignacio', 'Gómez', '38.452.197', NULL, NULL, 'Administrador', 1),
(2, 'empleado@inmobiliaria.com', 'AQAAAAIAAYagAAAAEJaWBM5W7LFs9FlsbXAyjF05+nwa3cu3fRwF6DDNdNJiV4ae6qncTzLT9pYYUFso/A==', NULL, 'Julián', ' Ramos', '41.894.822', NULL, NULL, 'Empleado', 1);
--
-- Índices para tablas volcadas
--

--
-- Indices de la tabla `inquilino`
--
ALTER TABLE `inquilino`
  ADD PRIMARY KEY (`ID_inquilino`),
  ADD UNIQUE KEY `dni` (`dni`);

--
-- Indices de la tabla `propietario`
--
ALTER TABLE `propietario`
  ADD PRIMARY KEY (`ID_propietario`),
  ADD UNIQUE KEY `dni` (`dni`);

--
-- Indices de la tabla `tipo_inmueble`
--
ALTER TABLE `tipo_inmueble`
  ADD PRIMARY KEY (`ID_tipo_inmueble`);

--
-- Indices de la tabla `inmueble`
--
ALTER TABLE `inmueble`
  ADD PRIMARY KEY (`ID_inmueble`),
  ADD KEY `FK_inmueble_tipo_inmueble` (`ID_tipo_inmueble`),
  ADD KEY `FK_inmueble_propietario` (`ID_propietario`);

--
-- Indices de la tabla `reserva`
--
ALTER TABLE `reserva`
  ADD PRIMARY KEY (`ID_reserva`),
  ADD KEY `ID_inmueble` (`ID_inmueble`),
  ADD KEY `ID_inquilino` (`ID_inquilino`),
  ADD KEY `ID_usuario_finalizador` (`ID_usuario_finalizador`),
  ADD KEY `ID_usuario_creador` (`ID_usuario_creador`);

--
-- Indices de la tabla `imagen`
--
ALTER TABLE `imagen`
  ADD PRIMARY KEY (`ID_imagen`),
  ADD KEY `FK_imagen_inmueble` (`ID_inmueble`);

--
-- Indices de la tabla `pago`
--
ALTER TABLE `pago`
  ADD PRIMARY KEY (`ID_pago`),
  ADD KEY `FK_pago_reserva` (`ID_reserva`),
  ADD KEY `ID_usuario_creador` (`ID_usuario_creador`),
  ADD KEY `ID_usuario_anulador` (`ID_usuario_anulador`);

--
-- Indices de la tabla `usuario`
--
ALTER TABLE `usuario`
  ADD PRIMARY KEY (`ID_usuario`);
--
-- AUTO_INCREMENT de las tablas volcadas
--

--
-- AUTO_INCREMENT de la tabla `inquilino`
--
ALTER TABLE `inquilino`
  MODIFY `ID_inquilino` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `propietario`
--
ALTER TABLE `propietario`
  MODIFY `ID_propietario` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `tipo_inmueble`
--
ALTER TABLE `tipo_inmueble`
  MODIFY `ID_tipo_inmueble` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `inmueble`
--
ALTER TABLE `inmueble`
  MODIFY `ID_inmueble` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `reserva`
--
ALTER TABLE `reserva`
  MODIFY `ID_reserva` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `imagen`
--
ALTER TABLE `imagen`
  MODIFY `ID_imagen` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `pago`
--
ALTER TABLE `pago`
  MODIFY `ID_pago` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `usuario`
--
ALTER TABLE `usuario`
  MODIFY `ID_usuario` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;
--
-- Restricciones para tablas volcadas
--

--
-- Filtros para la tabla `inmueble`
--
ALTER TABLE `inmueble`
  ADD CONSTRAINT `FK_inmueble_tipo_inmueble` FOREIGN KEY (`ID_tipo_inmueble`) REFERENCES `tipo_inmueble` (`ID_tipo_inmueble`),
  ADD CONSTRAINT `FK_inmueble_propietario` FOREIGN KEY (`ID_propietario`) REFERENCES `propietario` (`ID_propietario`);

--
-- Filtros para la tabla `reserva`
--
ALTER TABLE `reserva`
  ADD CONSTRAINT `reserva_ibfk_1` FOREIGN KEY (`ID_inmueble`) REFERENCES `inmueble` (`ID_inmueble`),
  ADD CONSTRAINT `reserva_ibfk_2` FOREIGN KEY (`ID_inquilino`) REFERENCES `inquilino` (`ID_inquilino`),
  ADD CONSTRAINT `reserva_ibfk_3` FOREIGN KEY (`ID_usuario_finalizador`) REFERENCES `usuario` (`ID_usuario`),
  ADD CONSTRAINT `reserva_ibfk_4` FOREIGN KEY (`ID_usuario_creador`) REFERENCES `usuario` (`ID_usuario`);

--
-- Filtros para la tabla `imagen`
--
ALTER TABLE `imagen`
  ADD CONSTRAINT `FK_imagen_inmueble` FOREIGN KEY (`ID_inmueble`) REFERENCES `inmueble` (`ID_inmueble`);

--
-- Filtros para la tabla `pago`
--
ALTER TABLE `pago`
  ADD CONSTRAINT `FK_pago_reserva` FOREIGN KEY (`ID_reserva`) REFERENCES `reserva` (`ID_reserva`),
  ADD CONSTRAINT `pago_ibfk_1` FOREIGN KEY (`ID_usuario_creador`) REFERENCES `usuario` (`ID_usuario`),
  ADD CONSTRAINT `pago_ibfk_2` FOREIGN KEY (`ID_usuario_anulador`) REFERENCES `usuario` (`ID_usuario`);

COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
