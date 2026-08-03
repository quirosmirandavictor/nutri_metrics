import { z } from "zod";

export const registerSchema = z
  .object({
    email: z.string().min(1, "El email es requerido").email("Email inválido"),
    password: z.string().min(8, "La contraseña debe tener al menos 8 caracteres"),
    passwordConfirm: z.string().min(1, "Confirmá la contraseña")
  })
  .refine((values) => values.password === values.passwordConfirm, {
    message: "Las contraseñas no coinciden",
    path: ["passwordConfirm"]
  });

export type RegisterFormValues = z.infer<typeof registerSchema>;
