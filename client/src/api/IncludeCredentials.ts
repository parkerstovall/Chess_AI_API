export abstract class IncludeCredentials {
  protected transformOptions(options: RequestInit) {
    options.credentials = 'include'
    return Promise.resolve(options)
  }
}
