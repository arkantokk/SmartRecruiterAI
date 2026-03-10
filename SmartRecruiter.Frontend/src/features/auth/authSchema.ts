import {z} from 'zod';

export const authSchema = z.object({
    email: z.email(),
    password: z.string().min(5, "Password must be longer than 5 symbols").max(100),
})

export type authFormValues = z.infer<typeof authSchema>;