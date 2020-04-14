import {
    TaskClient,
    Task,
    AuthClient,
    AuthToken,
} from "../../../client/js/lib/index";

let token: string;
const baseUrl = "http://localhost:8080";
const client = new TaskClient(baseUrl);

const task: Task = {
    answer: "42",
    question: "Meaning of life",
    taskId: "00",
    variants: ["Love", "Death", "42"],
};

async function authenticateAdmin(): Promise<AuthToken> {
    const client = new AuthClient(baseUrl);
    const authentication = await client.authenticateAsync("admin", "admin");
    return authentication.responseData;
}

async function createAndAuthenticateNonAdmin(): Promise<AuthToken> {
    const login = "not_an_admin_at_all";
    const password = "1234";

    const client = new AuthClient(baseUrl);
    await client.registerAsync({
        email: login,
        password: password,
        personalInfo: {
            address: "",
            birthday: new Date(),
            employer: "",
            name: "",
            occupation: "",
        },
    });

    const authentication = await client.authenticateAsync(login, password);

    return authentication.responseData;
}

beforeAll(async () => {
    token = await authenticateAdmin();
});

describe("task client", () => {
    it("creates new task", async () => {
        const taskCreation = await client.createTaskAsync(task, token);

        expect(taskCreation.responseCode).toBe(200);
    });

    it("finds created task", async () => {
        const taskCreation = await client.createTaskAsync(task, token);
        const taskId = taskCreation.responseData;

        setTimeout(async () => {
            const foundTask = await client.getTaskAsync(taskId, token);

            expect(foundTask.responseCode).toBe(200);
            expect(foundTask.responseData.answer).toBe(task.answer);
            expect(foundTask.responseData.question).toBe(task.question);
            expect(foundTask.responseData.variants).toStrictEqual(
                task.variants
            );
        }, 1000);
    });

    it("finds all created tasks", async () => {
        const foundTasks = await client.getAllTasksAsync(token);

        expect(foundTasks.responseCode).toBe(200);
        expect(foundTasks.responseData).toHaveLength(2);
    });

    it("gets 403 if not admin", async () => {
        const token = await createAndAuthenticateNonAdmin();

        const responses = [
            await client.getTaskAsync("some-id", token),
            await client.createTaskAsync(task, token),
            await client.getAllTasksAsync(token),
        ];

        expect(responses.map((r) => r.responseCode)).toStrictEqual([
            403,
            403,
            403,
        ]);
    });
});
