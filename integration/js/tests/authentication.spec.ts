import { AuthClient, User, PersonalInfo } from "../../../client/js/lib/index";

describe("authentication", () => {
    const email = "admin";
    const password = "admin";

    const personalInfo: PersonalInfo = {
        address: "Bakers st.",
        birthday: new Date(1970, 1, 1),
        employer: "G-man",
        name: "John Doe",
        occupation: "Entertainer",
    };

    const user: User = {
        email: email,
        password: password,
        personalInfo: personalInfo,
    };

    it("creates user", async () => {
        const client = new AuthClient("http://localhost:8080");
        const userCreationResponse = await client.registerAsync(user, 3000);

        expect(userCreationResponse.responseCode).toBe(200);
        expect(userCreationResponse.errorInfo).toBeUndefined();
        expect(userCreationResponse.responseData).toBeUndefined();
    });
});
