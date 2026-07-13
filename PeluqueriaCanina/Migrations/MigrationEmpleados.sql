-- ============================================================
-- SCRIPT DE MIGRACIÓN: Activar Empleados Existentes
-- ============================================================
-- Este script activa todos los empleados que fueron creados 
-- con IsActive = false durante la implementación anterior
-- 
-- IMPORTANTE: Ejecutar ANTES de que los empleados intenten
-- iniciar sesión
-- ============================================================

-- 1. Actualizar todos los empleados existentes con IsActive = false a true
UPDATE [AspNetUsers]
SET [IsActive] = 1
WHERE [TipoUsuario] = 'Empleado' AND [IsActive] = 0;

-- 2. Verificar cuántos usuarios fueron actualizados
SELECT 
	COUNT(*) as 'Empleados Activados',
	SUM(CASE WHEN [IsActive] = 1 THEN 1 ELSE 0 END) as 'Total Activos'
FROM [AspNetUsers]
WHERE [TipoUsuario] = 'Empleado';

-- 3. Listar todos los empleados activados
SELECT 
	[Id],
	[UserName],
	[Email],
	[TipoUsuario],
	[IsActive],
	[LockoutEnd]
FROM [AspNetUsers]
WHERE [TipoUsuario] = 'Empleado'
ORDER BY [UserName];

-- ============================================================
-- NOTAS FINALES:
-- ============================================================
-- - Los empleados ahora podrán iniciar sesión
-- - El admin puede desactivarlos desde el panel Admin > Empleados
-- - Cada empleado recibirá mensajes de error específicos si:
--   * Usuario no existe
--   * Contraseña es incorrecta
--   * Cuenta está desactivada
-- ============================================================
