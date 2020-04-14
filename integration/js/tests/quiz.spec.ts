import {
    QuizClient,
    AuthToken,
    AuthClient,
    Task,
    TaskId,
    TaskClient,
    User,
} from "../../../client/js/lib";

const baseUrl = "http://localhost:8080";
const client = new QuizClient(baseUrl);
const task: Task = {
    answer: "42",
    question: "Meaning of life",
    variants: ["Death", "Love", "42"],
    taskId: "",
};
let token: string;
const taskIds: TaskId[] = [];

async function authenticateAdminAsync(): Promise<AuthToken> {
    const authClient = new AuthClient(baseUrl);
    const authentication = await authClient.authenticateAsync("admin", "admin");

    return authentication.responseData;
}

async function prepareTasks(): Promise<void> {
    const taskClient = new TaskClient(baseUrl);
    const firstTask = await taskClient.createTaskAsync(task, token);
    const secondTask = await taskClient.createTaskAsync(task, token);

    taskIds.push(firstTask.responseData);
    taskIds.push(secondTask.responseData);
}

async function authenticateNotAdmin(): Promise<AuthToken> {
    const login = "1928734721";
    const password = "1203195791";
    const user: User = {
        email: login,
        password: password,
        personalInfo: {
            address: "",
            birthday: new Date(),
            employer: "",
            name: "",
            occupation: "",
        },
    };

    const client = new AuthClient(baseUrl);

    await client.registerAsync(user);

    const authentication = await client.authenticateAsync(login, password);

    return authentication.responseData;
}

beforeAll(async () => {
    token = await authenticateAdminAsync();
    await prepareTasks();
});

describe("quiz client", () => {
    it("creates new quiz", async () => {
        const quizCreation = await client.createQuizAsync(taskIds, token);

        expect(quizCreation.responseCode).toBe(200);
    });

    it("finds existing quiz", async () => {
        const quizCreation = await client.createQuizAsync(taskIds, token);
        const foundQuiz = await client.getQuizAsync(
            quizCreation.responseData.quizId,
            token
        );

        expect(foundQuiz.responseCode).toBe(200);
        expect(foundQuiz.responseData.quizId).toBe(
            quizCreation.responseData.quizId
        );
        expect(foundQuiz.responseData.tasks.map((t) => t.answer)).toStrictEqual(
            [task, task].map((t) => t.answer)
        );
        expect(
            foundQuiz.responseData.tasks.map((t) => t.question)
        ).toStrictEqual([task, task].map((t) => t.question));
        expect(
            foundQuiz.responseData.tasks.map((t) => t.variants)
        ).toStrictEqual([task, task].map((t) => t.variants));
    });

    it("finds all quizzes", async () => {
        const getAll = await client.getAllQuizzesAsync(token);

        expect(getAll.responseCode).toBe(200);
        expect(getAll.responseData).toHaveLength(2);
    });

    it("gets 403 if not admin", async () => {
        const nonAdminToken = await authenticateNotAdmin();

        const responses = [
            await client.getQuizAsync("some-id", nonAdminToken),
            await client.createQuizAsync([], nonAdminToken),
        ];

        expect(responses.map((r) => r.responseCode)).toStrictEqual([403, 403]);
    });
});
